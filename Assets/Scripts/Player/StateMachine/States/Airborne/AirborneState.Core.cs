using System;
using System.Collections.Generic;
using Assets.Scripts.Player.Data;
using Assets.Scripts.Player.StateMachine.States.Grounded;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States.Airborne
{
	public partial class AirborneState : IPlayerState
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

		private bool _fromCrouching = false;
		private bool _isSliding = false;
		private float _fallVelocity;
		private readonly float _slideGravityMultiplier;

		public void Enter(Dictionary<string, object> parameters)
		{
			PlayerContext.CurrentSuperState = PlayerSuperState.Airborne;

			PlayerContext.PlayerAnimator.SetInteger(
				PlayerAnimationHashes.AirborneSubState,
				(int)AirborneSubState.Ascending
			);

			SubscribeToEvents();
			HandleEntryParameters(parameters);
		}

		private void HandleEntryParameters(Dictionary<string, object> parameters)
		{
			if (parameters == null)
				return;

			if (parameters.GetBool(PlayerConstants.FROM_CROUCHING))
			{
				_fromCrouching = true;
				CurrentSubState = AirborneSubState.Falling;

				PlayerContext.PlayerAnimator.SetInteger(
					PlayerAnimationHashes.AirborneSubState,
					(int)AirborneSubState.Falling
				);
			}

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
							_jumpDirection = parameters
								.GetVector2(
									PlayerConstants.JUMP_DIRECTION,
									(Vector2)PlayerContext.GameObject.transform.forward
								)
								.normalized;

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
				HandleLanding();
				return;
			}

			HandleFalling();
			PlayerContext.CharacterController.Move(PlayerVelocity * Time.deltaTime);
		}

		public virtual void Exit()
		{
			UnsubscribeFromEvents();
		}

		private void EnterGlideMode()
		{
			_isGliding = true;
			_isLeap = false;
			_isSliding = false;
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

		private void HandleLanding()
		{
			Debug.Log("Landed! Transitioning to Grounded State.");
			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.IsGrounded, true);

			// 🚀 If gliding, always land standing, but determine if it's jogging or idle
			if (_isGliding)
			{
				TransitionToStandingBasedOnMovement();
				return;
			}

			if (_isSliding)
			{
				float surfaceFactor = IsOnSlope() ? 0.75f : 0.4f; // Faster on slopes, slower on flat ground
				float slideMomentum = Mathf.Abs(PlayerVelocity.y) * surfaceFactor;

				PlayerContext.StateMachine.TransitionTo(
					new SlidingState(),
					new Dictionary<string, object>
					{
						{ PlayerConstants.INITIAL_MOMENTUM, slideMomentum },
					}
				);
				return;
			}

			// 🚀 If leaping, transition to StandingState with special flag
			if (_isLeap)
			{
				PlayerContext.StateMachine.TransitionTo(
					new StandingState(),
					new Dictionary<string, object> { { PlayerConstants.FROM_LEAP, true } }
				);
				return;
			}

			// 🚀 If falling from a crouched state, land in CrouchingState
			if (_fromCrouching)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Crouching, true);
				_fromCrouching = false;

				PlayerContext.StateMachine.TransitionTo(
					new CrouchingState(),
					new Dictionary<string, object>
					{
						{
							InputMoveDirection.magnitude > 0
								? PlayerConstants.SNEAKING
								: PlayerConstants.CROUCHING_IDLE,
							true
						},
					}
				);
				return;
			}

			// 🚀 Default transition to StandingState based on movement
			TransitionToStandingBasedOnMovement();
		}

		/// <summary>
		/// Transitions to StandingState and determines if it's jogging or idle.
		/// </summary>
		private void TransitionToStandingBasedOnMovement()
		{
			PlayerContext.StateMachine.TransitionTo(
				new StandingState(),
				new Dictionary<string, object>
				{
					{
						InputMoveDirection.magnitude > 0
							? PlayerConstants.JOGGING
							: PlayerConstants.STANDING_IDLE,
						true
					},
				}
			);
		}

		private bool IsOnSlope()
		{
			throw new NotImplementedException();
		}

		private void HandleFalling()
		{
			if (_isGliding)
			{
				PlayerVelocity.y = Mathf.Lerp(
					PlayerVelocity.y,
					_glideFallSpeedLimit,
					Time.deltaTime * _glideSmoothFactor
				);
			}
			else if (_isSliding)
			{
				_fallVelocity += _slideGravityMultiplier * Time.deltaTime;
			}
			else
			{
				PlayerVelocity.y += Physics.gravity.y * _gravityMultiplier * Time.deltaTime;
			}
		}
	}
}
