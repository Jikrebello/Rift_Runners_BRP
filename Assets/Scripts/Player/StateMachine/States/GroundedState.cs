using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States
{
	public abstract class GroundedState : IPlayerState
	{
		public PlayerContext PlayerContext { get; set; }
		protected GroundedSubState CurrentSubState { get; set; }

		protected Vector2 InputMoveDirection;

		private readonly float _jumpMagnitude = 5;
		private readonly float _leapMagnitude = 7;
		private readonly float _superJumpMagnitude = 10;
		private readonly float _longJumpMagnitude = 10;
		private readonly float _superJumpChargeTime = 1.0f;

		private float _currentChargeTime = 0f;
		private bool _isChargingJump = false;
		private Vector2 _storedJumpDirection = Vector2.zero;

		public virtual void Enter(Dictionary<string, object> parameters)
		{
			PlayerContext.CurrentSuperState = PlayerSuperState.Grounded;

			PlayerContext.PlayerInputEvents.MoveEvent += HandleMove;
			PlayerContext.PlayerInputEvents.ToggleCrouchEvent += HandleToggleCrouch;
			PlayerContext.PlayerInputEvents.ToggleSprintEvent += HandleToggleSprint;
			PlayerContext.PlayerInputEvents.ToggleWeaponStanceEvent += HandleToggleWeaponStance;
			PlayerContext.PlayerInputEvents.JumpEvent += HandleJump;
			PlayerContext.PlayerInputEvents.JumpCancelledEvent += HandleJumpCancelled;
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
			PlayerContext.PlayerInputEvents.MoveEvent -= HandleMove;
			PlayerContext.PlayerInputEvents.ToggleCrouchEvent -= HandleToggleCrouch;
			PlayerContext.PlayerInputEvents.ToggleSprintEvent -= HandleToggleSprint;
			PlayerContext.PlayerInputEvents.ToggleWeaponStanceEvent -= HandleToggleWeaponStance;
			PlayerContext.PlayerInputEvents.JumpEvent -= HandleJump;
			PlayerContext.PlayerInputEvents.JumpCancelledEvent -= HandleJumpCancelled;
		}

		private void HandleMove(Vector2 direction)
		{
			InputMoveDirection = direction;
		}

		private void HandleToggleCrouch()
		{
			if (CurrentSubState == GroundedSubState.Sprinting)
			{
				CurrentSubState = GroundedSubState.Standing;
				return;
			}

			if (_isChargingJump)
			{
				_isChargingJump = false;
				_currentChargeTime = 0f;
				Debug.Log("Super Jump Cancelled (Crouch Released)");
			}

			CurrentSubState =
				(CurrentSubState == GroundedSubState.Crouching)
					? GroundedSubState.Standing
					: GroundedSubState.Crouching;
		}

		private void HandleToggleSprint()
		{
			if (CurrentSubState == GroundedSubState.Sprinting)
			{
				CurrentSubState = GroundedSubState.Standing;
				return;
			}

			CurrentSubState = GroundedSubState.Sprinting;
			PlayerContext.CurrentCombatStance = PlayerCombatStance.Passive;
		}

		private void HandleToggleWeaponStance()
		{
			if (CurrentSubState == GroundedSubState.Sprinting)
				return;

			PlayerContext.CurrentCombatStance =
				(PlayerContext.CurrentCombatStance == PlayerCombatStance.Engaged)
					? PlayerCombatStance.Passive
					: PlayerCombatStance.Engaged;
		}

		private void HandleJump()
		{
			if (CurrentSubState == GroundedSubState.Crouching)
			{
				_isChargingJump = true;
				_currentChargeTime = 0f;
				_storedJumpDirection = InputMoveDirection;
				return;
			}

			if (InputMoveDirection.magnitude > 0 && CurrentSubState == GroundedSubState.Sprinting)
			{
				PlayerContext.StateMachine.TransitionTo(
					new AirborneState(),
					new Dictionary<string, object> { { PlayerConstants.LEAP, _leapMagnitude } }
				);
				return;
			}

			if (CurrentSubState == GroundedSubState.Standing)
			{
				PlayerContext.StateMachine.TransitionTo(
					new AirborneState(),
					new Dictionary<string, object> { { PlayerConstants.JUMP, _jumpMagnitude } }
				);
				return;
			}
		}

		private void HandleJumpCancelled()
		{
			if (!_isChargingJump)
			{
				if (CurrentSubState == GroundedSubState.Crouching)
				{
					CurrentSubState = GroundedSubState.Standing;
				}
				return;
			}

			_isChargingJump = false;
			Debug.Log("Jump Released!");

			if (_currentChargeTime >= _superJumpChargeTime)
			{
				if (_storedJumpDirection.magnitude == 0)
				{
					PlayerContext.StateMachine.TransitionTo(
						new AirborneState(),
						new Dictionary<string, object>
						{
							{ PlayerConstants.SUPER_JUMP, _superJumpMagnitude },
						}
					);
				}
				else
				{
					PlayerContext.StateMachine.TransitionTo(
						new AirborneState(),
						new Dictionary<string, object>
						{
							{ PlayerConstants.LONG_JUMP, _longJumpMagnitude },
							{ "jumpDirection", _storedJumpDirection },
						}
					);
				}
			}
			else
			{
				CurrentSubState = GroundedSubState.Standing;
			}

			_currentChargeTime = 0f;
			_storedJumpDirection = Vector2.zero;
		}
	}
}
