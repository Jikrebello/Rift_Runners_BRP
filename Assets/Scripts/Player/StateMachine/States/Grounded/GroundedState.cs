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

		public virtual void Update() { }

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

		protected virtual void HandleToggleCrouchEvent()
		{
			if (CurrentGroundedSubState == GroundedSubState.Sprinting)
			{
				CurrentGroundedSubState = GroundedSubState.Standing;
				return;
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
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sprinting, false);
				return;
			}

			CurrentGroundedSubState = GroundedSubState.Sprinting;
			PlayerContext.CurrentCombatStance = PlayerCombatStance.Holstered;
		}

		private void HandleToggleWeaponStanceEvent()
		{
			if (CurrentGroundedSubState == GroundedSubState.Sprinting)
				return;

			bool isCurrentlyHolstered =
				PlayerContext.CurrentCombatStance == PlayerCombatStance.Holstered;

			PlayerContext.CurrentCombatStance = isCurrentlyHolstered
				? PlayerCombatStance.Unholstered
				: PlayerCombatStance.Holstered;

			PlayerContext.PlayerAnimator.SetBool(
				PlayerAnimationHashes.Holstered,
				isCurrentlyHolstered
			);
		}

		protected virtual void HandleJumpEvent()
		{
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

		protected virtual void HandleJumpCancelledEvent() { }
	}
}
