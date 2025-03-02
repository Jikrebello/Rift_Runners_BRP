using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States
{
	public class AirborneState : IPlayerState
	{
		public PlayerContext PlayerContext { get; set; }

		protected AirborneSubState CurrentSubState { get; set; }

		protected Vector2 InputMoveDirection;
		protected Vector3 PlayerVelocity;

		private Vector2 _jumpDirection = Vector2.zero;
		private float _jumpForce = 0f;

		private readonly float _doubleJumpMagnitude = 5;

		private bool _canDoubleJump = true;
		private bool _isGliding = false;
		private bool _isLeap = false;
		private bool _isHoldingJump = false;

		private readonly float _glideFallSpeedLimit = -1.5f;
		private readonly float _glideSmoothFactor = 5f;
		private readonly float _gravityMultiplier = 1f;
		private readonly float _glideSpeedMultiplier = 1.1f;

		public void Enter(Dictionary<string, object> parameters)
		{
			PlayerContext.CurrentSuperState = PlayerSuperState.Airborne;
			HandleEntryParameters(parameters);
			PlayerContext.PlayerInputEvents.MoveEvent += HandleMoveEvent;
			PlayerContext.PlayerInputEvents.JumpEvent += HandleJumpEvent;
			PlayerContext.PlayerInputEvents.JumpHeldEvent += HandleJumpHeldEvent;
			PlayerContext.PlayerInputEvents.JumpCancelledEvent += HandleJumpCancelledEvent;
		}

		private void HandleEntryParameters(Dictionary<string, object> parameters)
		{
			if (parameters == null)
				return;

			if (parameters.TryGetValue(PlayerConstants.JUMP, out object jumpMagnitude))
			{
				HandleNormalJumpEntry((float)jumpMagnitude);
			}
			else if (parameters.TryGetValue(PlayerConstants.LEAP, out object leapMagnitude))
			{
				HandleSprintLeapEntry((float)leapMagnitude);
			}
			else if (
				parameters.TryGetValue(PlayerConstants.SUPER_JUMP, out object superJumpMagnitude)
			)
			{
				HandleSuperJumpEntry((float)superJumpMagnitude);
			}
			else if (
				parameters.TryGetValue(PlayerConstants.LONG_JUMP, out object longJumpMagnitude)
			)
			{
				Vector2 direction = parameters.ContainsKey("jumpDirection")
					? (Vector2)parameters["jumpDirection"]
					: (Vector2)PlayerContext.GameObject.transform.forward; // TODO: Make this the direction of the player model

				HandleLongJumpEntry((float)longJumpMagnitude, direction);
			}
		}

		public virtual void Update()
		{
			if (
				CurrentSubState == AirborneSubState.Ascending
				&& PlayerContext.CharacterController.velocity.y <= 0
			)
			{
				CurrentSubState = AirborneSubState.Falling;
				Debug.Log("Transitioning to Falling");
			}

			if (_isHoldingJump && CurrentSubState == AirborneSubState.Falling && !_isGliding)
			{
				EnterGlideMode();
			}
		}

		public virtual void FixedUpdate()
		{
			if (PlayerContext.CharacterController.isGrounded)
			{
				Debug.Log("Landed! Transitioning to Grounded State.");

				if (_isLeap)
				{
					PlayerContext.StateMachine.TransitionTo(
						new WalkingState(),
						new Dictionary<string, object> { { "fromLeap", true } }
					);
					return;
				}

				switch (InputMoveDirection.magnitude)
				{
					case > 0:
						PlayerContext.StateMachine.TransitionTo(new WalkingState());
						break;
					default:
						PlayerContext.StateMachine.TransitionTo(new IdleState());
						break;
				}

				return;
			}

			if (!PlayerContext.CharacterController.isGrounded)
			{
				if (_isGliding)
				{
					PlayerVelocity.y = Mathf.Lerp(
						PlayerVelocity.y,
						_glideFallSpeedLimit,
						Time.deltaTime * _glideSmoothFactor
					);
				}
				else
				{
					PlayerVelocity.y += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
				}
			}

			PlayerContext.CharacterController.Move(PlayerVelocity * Time.deltaTime);
		}

		public virtual void Exit()
		{
			PlayerContext.PlayerInputEvents.MoveEvent -= HandleMoveEvent;
			PlayerContext.PlayerInputEvents.JumpEvent -= HandleJumpEvent;
			PlayerContext.PlayerInputEvents.JumpHeldEvent -= HandleJumpHeldEvent;
			PlayerContext.PlayerInputEvents.JumpCancelledEvent -= HandleJumpCancelledEvent;
		}

		private void HandleNormalJumpEntry(float jumpMagnitude)
		{
			_jumpForce = jumpMagnitude;
			PlayerVelocity.y = _jumpForce;
			CurrentSubState = AirborneSubState.Ascending;
		}

		private void HandleSprintLeapEntry(float leapMagnitude)
		{
			_jumpForce = leapMagnitude;
			_isLeap = true;
			_jumpDirection = PlayerContext.PlayerTransform.forward;
			CurrentSubState = AirborneSubState.Ascending;
			Debug.Log($"Sprint Leap - Force: {_jumpForce}, Direction: {_jumpDirection}");
		}

		private void HandleSuperJumpEntry(float superJumpMagnitude)
		{
			_jumpForce = superJumpMagnitude;
			CurrentSubState = AirborneSubState.Ascending;
			Debug.Log($"Super Jump - Force: {_jumpForce}");
		}

		private void HandleLongJumpEntry(float longJumpMagnitude, Vector2 direction)
		{
			_jumpForce = longJumpMagnitude;
			_jumpDirection = direction.normalized;
			CurrentSubState = AirborneSubState.Ascending;
			Debug.Log($"Long Jump - Force: {_jumpForce}, Direction: {_jumpDirection}");
		}

		private void HandleMoveEvent(Vector2 direction)
		{
			InputMoveDirection = direction.normalized;
		}

		private void HandleJumpEvent()
		{
			if (_canDoubleJump)
			{
				PlayerVelocity.y = _doubleJumpMagnitude;
				_canDoubleJump = false;
				_isLeap = false;
				CurrentSubState = AirborneSubState.Ascending;

				Debug.Log("Double Jump Triggered");
			}
		}

		private void HandleJumpHeldEvent()
		{
			_isHoldingJump = true;

			if (CurrentSubState == AirborneSubState.Falling && !_isGliding)
			{
				EnterGlideMode();
			}
		}

		private void HandleJumpCancelledEvent()
		{
			_isHoldingJump = false;

			if (_isGliding)
			{
				_isGliding = false;
				CurrentSubState = AirborneSubState.Falling;
				Debug.Log("Stopped Gliding");
			}
		}

		private void EnterGlideMode()
		{
			_isGliding = true;
			_isLeap = false;
			CurrentSubState = AirborneSubState.Gliding;

			PlayerVelocity.y = Mathf.Max(PlayerVelocity.y, _glideFallSpeedLimit);
			PlayerVelocity.x *= _glideSpeedMultiplier;
			PlayerVelocity.z *= _glideSpeedMultiplier;

			Debug.Log("Entering Glide Mode");
		}
	}
}
