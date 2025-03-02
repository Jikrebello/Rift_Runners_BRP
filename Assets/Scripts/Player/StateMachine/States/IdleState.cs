using System.Collections.Generic;

namespace Assets.Scripts.Player.StateMachine.States
{
	public class IdleState : GroundedState
	{
		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);

			CurrentSubState = GroundedSubState.Standing;
		}

		public override void Update()
		{
			base.Update();

			if (InputMoveDirection.magnitude > 0)
			{
				PlayerContext.StateMachine.TransitionTo(
					new WalkingState(),
					new Dictionary<string, object>
					{
						{ "previousSpeed", InputMoveDirection.magnitude },
					}
				);
			}

			if (InputMoveDirection.magnitude > 0 && CurrentSubState == GroundedSubState.Sprinting)
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
