using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States
{
	public class SprintingState : GroundedState
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
			else if (InputMoveDirection.magnitude > 0)
			{
				PlayerContext.StateMachine.TransitionTo(
					new WalkingState(),
					new Dictionary<string, object>
					{
						{ "previousSpeed", InputMoveDirection.magnitude },
					}
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
