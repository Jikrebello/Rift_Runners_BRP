using System.Collections.Generic;
using Assets.Scripts.Player.Data;
using Assets.Scripts.Player.StateMachine.States.Grounded;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States.Airborne
{
	public class AirborneState : IPlayerState
	{
		public PlayerContext PlayerContext { get; set; }

		protected AirborneSubState CurrentSubState { get; set; }

		protected Vector2 InputMoveDirection;
		protected Vector3 PlayerVelocity;

		private readonly float _jumpMagnitude = 5;
		private readonly float _leapMagnitude = 10;
		private readonly float _superJumpMagnitude = 10;
		private readonly float _longJumpMagnitude = 10;

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

			PlayerContext.PlayerAnimator.SetInteger(
				PlayerAnimationHashes.AirborneSubState,
				(int)AirborneSubState.Ascending
			);

			PlayerContext.PlayerInputEvents.MoveEvent += HandleMoveEvent;
			PlayerContext.PlayerInputEvents.JumpEvent += HandleJumpEvent;
			PlayerContext.PlayerInputEvents.JumpHeldEvent += HandleJumpHeldEvent;
			PlayerContext.PlayerInputEvents.JumpCancelledEvent += HandleJumpCancelledEvent;

			HandleEntryParameters(parameters);
		}

		private void HandleEntryParameters(Dictionary<string, object> parameters)
		{
			if (parameters == null)
				return;

			var jumpMappings = new Dictionary<string, (float magnitude, bool setDirection)>
			{
				{ PlayerConstants.JUMP, (_jumpMagnitude, false) },
				{ PlayerConstants.LEAP, (_leapMagnitude, true) },
				{ PlayerConstants.SUPER_JUMP, (_superJumpMagnitude, false) },
				{ PlayerConstants.LONG_JUMP, (_longJumpMagnitude, true) },
			};

			foreach (var (key, (magnitude, setDirection)) in jumpMappings)
			{
				if (parameters.TryGetValue(key, out object value) && value is bool isJump && isJump)
				{
					_jumpForce = magnitude;
					CurrentSubState = AirborneSubState.Ascending;

					if (setDirection)
					{
						if (key == PlayerConstants.LEAP)
						{
							_isLeap = true;
							_jumpDirection = PlayerContext.PlayerTransform.forward;
							Debug.Log(
								$"Sprint Leap - Force: {_jumpForce}, Direction: {_jumpDirection}"
							);
						}
						else if (key == PlayerConstants.LONG_JUMP)
						{
							_jumpDirection = parameters.ContainsKey("jumpDirection")
								? ((Vector2)parameters["jumpDirection"]).normalized
								: (Vector2)PlayerContext.GameObject.transform.forward; // TODO: Make this the direction of the player model
							Debug.Log(
								$"Long Jump - Force: {_jumpForce}, Direction: {_jumpDirection}"
							);
						}
					}
					else if (key == PlayerConstants.SUPER_JUMP)
					{
						Debug.Log($"Super Jump - Force: {_jumpForce}");
					}

					PlayerVelocity.y = _jumpForce;
					break;
				}
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
				PlayerContext.PlayerAnimator.SetInteger(
					PlayerAnimationHashes.AirborneSubState,
					(int)AirborneSubState.Falling
				);
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

				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.IsGrounded, true);

				if (_isLeap)
				{
					PlayerContext.StateMachine.TransitionTo(
						new StandingState(),
						new Dictionary<string, object> { { PlayerConstants.FROM_LEAP, true } }
					);
					return;
				}

				switch (InputMoveDirection.magnitude)
				{
					case > 0:
						PlayerContext.StateMachine.TransitionTo(
							new StandingState(),
							new Dictionary<string, object> { { PlayerConstants.JOGGING, true } }
						);
						break;
					default:
						PlayerContext.StateMachine.TransitionTo(
							new StandingState(),
							new Dictionary<string, object>
							{
								{ PlayerConstants.STANDING_IDLE, true },
							}
						);
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

		private void HandleMoveEvent(Vector2 direction)
		{
			InputMoveDirection = direction;
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

			PlayerContext.PlayerAnimator.SetInteger(
				PlayerAnimationHashes.AirborneSubState,
				(int)AirborneSubState.Gliding
			);

			PlayerVelocity.y = Mathf.Max(PlayerVelocity.y, _glideFallSpeedLimit);
			PlayerVelocity.x *= _glideSpeedMultiplier;
			PlayerVelocity.z *= _glideSpeedMultiplier;

			Debug.Log("Entering Glide Mode");
		}
	}
}
