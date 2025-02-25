using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States
{
	public abstract class GroundedState : IPlayerState
	{
		public PlayerContext PlayerContext { get; set; }
		protected GroundedSubStates CurrentSubState { get; set; }

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
			if (CurrentSubState == GroundedSubStates.Sprinting)
			{
				CurrentSubState = GroundedSubStates.Standing;
				return;
			}

			if (_isChargingJump)
			{
				_isChargingJump = false;
				_currentChargeTime = 0f;
				Debug.Log("Super Jump Cancelled (Crouch Released)");
			}

			CurrentSubState =
				(CurrentSubState == GroundedSubStates.Crouching)
					? GroundedSubStates.Standing
					: GroundedSubStates.Crouching;
		}

		private void HandleToggleSprint()
		{
			if (CurrentSubState == GroundedSubStates.Sprinting)
			{
				CurrentSubState = GroundedSubStates.Standing;
				return;
			}

			CurrentSubState = GroundedSubStates.Sprinting;
			PlayerContext.CurrentCombatStance = PlayerCombatStance.Passive;
		}

		private void HandleToggleWeaponStance()
		{
			if (CurrentSubState == GroundedSubStates.Sprinting)
				return;

			PlayerContext.CurrentCombatStance =
				(PlayerContext.CurrentCombatStance == PlayerCombatStance.Engaged)
					? PlayerCombatStance.Passive
					: PlayerCombatStance.Engaged;
		}

		private void HandleJump()
		{
			if (CurrentSubState == GroundedSubStates.Crouching)
			{
				_isChargingJump = true;
				_currentChargeTime = 0f;
				_storedJumpDirection = InputMoveDirection;
				return;
			}

			if (InputMoveDirection.magnitude > 0 && CurrentSubState == GroundedSubStates.Sprinting)
			{
				PlayerContext.StateMachine.TransitionTo(
					new AirborneState(),
					new Dictionary<string, object> { { "leap", _leapMagnitude } }
				);
				return;
			}

			if (CurrentSubState == GroundedSubStates.Standing)
			{
				PlayerContext.StateMachine.TransitionTo(
					new AirborneState(),
					new Dictionary<string, object> { { "jump", _jumpMagnitude } }
				);
				return;
			}
		}

		private void HandleJumpCancelled()
		{
			if (!_isChargingJump)
				return;

			_isChargingJump = false;
			Debug.Log("Jump Released!");

			if (_currentChargeTime >= _superJumpChargeTime)
			{
				if (_storedJumpDirection.magnitude == 0)
				{
					PlayerContext.StateMachine.TransitionTo(
						new AirborneState(),
						new Dictionary<string, object> { { "superJump", _superJumpMagnitude } }
					);
				}
				else
				{
					PlayerContext.StateMachine.TransitionTo(
						new AirborneState(),
						new Dictionary<string, object>
						{
							{ "longJump", _longJumpMagnitude },
							{ "jumpDirection", _storedJumpDirection },
						}
					);
				}
			}
			else
			{
				PlayerContext.StateMachine.TransitionTo(
					new AirborneState(),
					new Dictionary<string, object> { { "jump", _jumpMagnitude } }
				);
			}

			_currentChargeTime = 0f;
			_storedJumpDirection = Vector2.zero;
		}
	}
}
