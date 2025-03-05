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
		}

		public override void Update()
		{
			base.Update();

			// TODO: Needs further fleshing out when airborne is sorted out, fine as is for now

			if (InputMoveDirection.magnitude == 0)
			{
				PlayerContext.StateMachine.TransitionTo(new StandingState());
			}
			else if (InputMoveDirection.magnitude > 0)
			{
				PlayerContext.StateMachine.TransitionTo(new StandingState());
			}
		}

		public override void FixedUpdate() { }

		public override void Exit()
		{
			base.Exit();
		}
	}
}
