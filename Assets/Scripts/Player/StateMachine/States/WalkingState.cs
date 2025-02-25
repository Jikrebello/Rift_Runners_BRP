using System.Collections.Generic;

namespace Assets.Scripts.Player.StateMachine.States
{
	public class WalkingState : GroundedState
	{
		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);
		}

		public override void Update()
		{
			base.Update();

			if (InputMoveDirection.magnitude == 0)
			{
				PlayerContext.StateMachine.TransitionTo(
					new IdleState(),
					new Dictionary<string, object> { { "previousSpeed", 0 } }
				);
			}

			if (InputMoveDirection.magnitude > 0 && CurrentSubState == GroundedSubStates.Sprinting)
			{
				PlayerContext.StateMachine.TransitionTo(new SprintingState());
			}
		}

		public override void FixedUpdate() { }

		public override void Exit()
		{
			base.Exit();
		}
	}
}
