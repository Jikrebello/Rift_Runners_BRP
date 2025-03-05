using System.Collections.Generic;
using Assets.Scripts.Player.Data;
using Assets.Scripts.Player.StateMachine.States.Airborne;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States.Grounded
{
	public abstract class GroundedState : IPlayerState
	{
		public PlayerContext PlayerContext { get; set; }
		protected GroundedSubState CurrentGroundedSubState { get; set; }

		protected Vector2 InputMoveDirection;

		private readonly float _superJumpChargeTime = 1.0f;

		private float _currentChargeTime = 0f;
		private bool _isChargingJump = false;
		private Vector2 _storedJumpDirection = Vector2.zero;

		public virtual void Enter(Dictionary<string, object> parameters)
		{
			PlayerContext.CurrentSuperState = PlayerSuperState.Grounded;

			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.IsGrounded, true);

			PlayerContext.PlayerInputEvents.MoveEvent += HandleMoveEvent;
			PlayerContext.PlayerInputEvents.ToggleCrouchEvent += HandleToggleCrouchEvent;
			PlayerContext.PlayerInputEvents.ToggleSprintEvent += HandleToggleSprintEvent;
			PlayerContext.PlayerInputEvents.ToggleWeaponStanceEvent +=
				HandleToggleWeaponStanceEvent;
			PlayerContext.PlayerInputEvents.JumpEvent += HandleJumpEvent;
			PlayerContext.PlayerInputEvents.JumpCancelledEvent += HandleJumpCancelledEvent;
		}

		public virtual void Update()
		{
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
		}

		public abstract void FixedUpdate();

		public virtual void Exit()
		{
			PlayerContext.PlayerInputEvents.MoveEvent -= HandleMoveEvent;
			PlayerContext.PlayerInputEvents.ToggleCrouchEvent -= HandleToggleCrouchEvent;
			PlayerContext.PlayerInputEvents.ToggleSprintEvent -= HandleToggleSprintEvent;
			PlayerContext.PlayerInputEvents.ToggleWeaponStanceEvent -=
				HandleToggleWeaponStanceEvent;
			PlayerContext.PlayerInputEvents.JumpEvent -= HandleJumpEvent;
			PlayerContext.PlayerInputEvents.JumpCancelledEvent -= HandleJumpCancelledEvent;
		}

		private void HandleMoveEvent(Vector2 direction)
		{
			InputMoveDirection = direction;
		}

		private void HandleToggleCrouchEvent()
		{
			if (CurrentGroundedSubState == GroundedSubState.Sprinting)
			{
				CurrentGroundedSubState = GroundedSubState.Standing;
				return;
			}

			if (_isChargingJump)
			{
				_isChargingJump = false;
				_currentChargeTime = 0f;
				Debug.Log("Super Jump Cancelled (Crouch Released)");
			}

			CurrentGroundedSubState =
				CurrentGroundedSubState == GroundedSubState.Crouching
					? GroundedSubState.Standing
					: GroundedSubState.Crouching;
		}

		private void HandleToggleSprintEvent()
		{
			if (CurrentGroundedSubState == GroundedSubState.Sprinting)
			{
				CurrentGroundedSubState = GroundedSubState.Standing;
				return;
			}

			CurrentGroundedSubState = GroundedSubState.Sprinting;
			PlayerContext.CurrentCombatStance = PlayerCombatStance.Passive;
		}

		private void HandleToggleWeaponStanceEvent()
		{
			if (CurrentGroundedSubState == GroundedSubState.Sprinting)
				return;

			PlayerContext.CurrentCombatStance =
				PlayerContext.CurrentCombatStance == PlayerCombatStance.Engaged
					? PlayerCombatStance.Passive
					: PlayerCombatStance.Engaged;
		}

		private void HandleJumpEvent()
		{
			if (CurrentGroundedSubState == GroundedSubState.Crouching)
			{
				_isChargingJump = true;
				_currentChargeTime = 0f;
				_storedJumpDirection = InputMoveDirection;
				return;
			}

			if (
				InputMoveDirection.magnitude > 0
				&& CurrentGroundedSubState == GroundedSubState.Sprinting
			)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.IsGrounded, false);

				PlayerContext.StateMachine.TransitionTo(
					new AirborneState(),
					new Dictionary<string, object> { { PlayerConstants.LEAP, true } }
				);
				return;
			}

			if (CurrentGroundedSubState == GroundedSubState.Standing)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.IsGrounded, false);

				PlayerContext.StateMachine.TransitionTo(
					new AirborneState(),
					new Dictionary<string, object> { { PlayerConstants.JUMP, true } }
				);
				return;
			}
		}

		private void HandleJumpCancelledEvent()
		{
			if (!_isChargingJump)
			{
				if (CurrentGroundedSubState == GroundedSubState.Crouching)
				{
					CurrentGroundedSubState = GroundedSubState.Standing;
				}
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
							{ "jumpDirection", _storedJumpDirection },
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
	}
}
