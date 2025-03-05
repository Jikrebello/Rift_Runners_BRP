using System.Collections.Generic;
using Assets.Scripts.Player.Data;

namespace Assets.Scripts.Player.StateMachine.States.Grounded
{
	public class SprintingState : GroundedState
	{
		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);

			CurrentGroundedSubState = GroundedSubState.Sprinting;

			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sprinting, true);
		}

		public override void Update()
		{
			base.Update();

			// TODO: Needs further fleshing out when airborne is sorted out, fine as is for now

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
	}
}
