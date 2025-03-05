using System.Collections.Generic;
using Assets.Scripts.Player.Data;
using Assets.Scripts.Player.StateMachine.States.Airborne;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States.Grounded
{
	class CrouchingState : GroundedState
	{
		private CrouchingSubState _currentCrouchSubState = new();
		private float _blendValue;

		private readonly float _superJumpChargeTime = 1.0f;

		private float _currentChargeTime = 0f;
		private bool _isChargingJump = false;
		private Vector2 _storedJumpDirection = Vector2.zero;

		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);

			CurrentGroundedSubState = GroundedSubState.Crouching;

			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Crouching, true);
		}

		public override void Update()
		{
			base.Update();

			if (_isChargingJump)
			{
				_currentChargeTime += Time.deltaTime;

				if (
					_currentChargeTime >= _superJumpChargeTime
					&& _currentChargeTime - Time.deltaTime < _superJumpChargeTime
				)
				{
					Debug.Log("Super Jump Fully Charged!");
				}
			}

			ProcessAnimations();

			HandleStateTransitions();
		}

		public override void FixedUpdate() { }

		public override void Exit()
		{
			base.Exit();
		}

		protected override void HandleToggleCrouchEvent()
		{
			base.HandleToggleCrouchEvent();

			if (_isChargingJump)
			{
				_isChargingJump = false;
				_currentChargeTime = 0f;
				Debug.Log("Super Jump Cancelled (Crouch Released)");
			}
		}

		protected override void HandleJumpEvent()
		{
			_isChargingJump = true;
			_currentChargeTime = 0f;
			_storedJumpDirection = InputMoveDirection;

			base.HandleJumpEvent();
		}

		protected override void HandleJumpCancelledEvent()
		{
			base.HandleJumpCancelledEvent();

			if (!_isChargingJump)
			{
				CurrentGroundedSubState = GroundedSubState.Standing;
				return;
			}

			_isChargingJump = false;
			Debug.Log("Jump Released!");

			if (_currentChargeTime >= _superJumpChargeTime)
			{
				if (_storedJumpDirection.magnitude == 0)
				{
					PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.IsGrounded, false);

					PlayerContext.StateMachine.TransitionTo(
						new AirborneState(),
						new Dictionary<string, object> { { PlayerConstants.SUPER_JUMP, true } }
					);
				}
				else
				{
					PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.IsGrounded, false);

					PlayerContext.StateMachine.TransitionTo(
						new AirborneState(),
						new Dictionary<string, object>
						{
							{ PlayerConstants.LONG_JUMP, true },
							{ PlayerConstants.JUMP_DIRECTION, _storedJumpDirection },
						}
					);
				}
			}
			else
			{
				CurrentGroundedSubState = GroundedSubState.Standing;
			}

			_currentChargeTime = 0f;
			_storedJumpDirection = Vector2.zero;
		}

		private void ProcessAnimations()
		{
			_blendValue = InputMoveDirection.magnitude;

			PlayerContext.PlayerAnimator.SetFloat(PlayerAnimationHashes.Speed, _blendValue);

			switch (_blendValue)
			{
				case 0:
					_currentCrouchSubState = CrouchingSubState.Idle;
					break;

				case >= 0.1f:
					_currentCrouchSubState = CrouchingSubState.Sneaking;
					break;
			}
		}

		private void HandleStateTransitions()
		{
			if (!PlayerContext.CharacterController.isGrounded)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.IsGrounded, false);
				PlayerContext.PlayerAnimator.SetInteger(
					PlayerAnimationHashes.AirborneSubState,
					(int)AirborneSubState.Falling
				);

				PlayerContext.StateMachine.TransitionTo(
					new AirborneState(),
					new Dictionary<string, object> { { PlayerConstants.FROM_CROUCHING, true } }
				);
				return;
			}

			if (CurrentGroundedSubState == GroundedSubState.Standing)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Crouching, false);
				PlayerContext.StateMachine.TransitionTo(new StandingState());
			}
		}
	}
}
