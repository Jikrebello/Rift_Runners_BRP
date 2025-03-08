using System;
using System.Collections.Generic;
using Assets.Scripts.Player.Data;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States.Grounded
{
	public class SprintingState : GroundedState
	{
		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);

			CurrentGroundedSubState = GroundedSubState.Sprinting;
			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sprinting, true);

			PlayerContext.PlayerInputEvents.SlideEvent += HandleSlideEvent;
		}

		public override void Update()
		{
			base.Update();

			if (InputMoveDirection.magnitude == 0)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sprinting, false);

				PlayerContext.StateMachine.TransitionTo(
					new StandingState(),
					new Dictionary<string, object> { { PlayerConstants.STANDING_IDLE, true } }
				);
			}
			else if (
				InputMoveDirection.magnitude > 0
				&& CurrentGroundedSubState == GroundedSubState.Standing
			)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sprinting, false);

				PlayerContext.StateMachine.TransitionTo(
					new StandingState(),
					new Dictionary<string, object> { { PlayerConstants.JOGGING, true } }
				);
			}
		}

		public override void FixedUpdate() { }

		public override void Exit()
		{
			base.Exit();
		}

		private void HandleSlideEvent()
		{
			if (InputMoveDirection.magnitude > 0)
			{
				Debug.Log("Transitioning to Slide!");
				PlayerContext.StateMachine.TransitionTo(
					new SlidingState(),
					new Dictionary<string, object> { { PlayerConstants.INITIAL_MOMENTUM, 5.0f } }
				);
			}
		}
	}
}
