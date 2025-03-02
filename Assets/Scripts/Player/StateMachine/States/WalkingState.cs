using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States
{
	public class WalkingState : GroundedState
	{
		private bool _isRolling = false;

		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);

			CurrentSubState = GroundedSubState.Standing;

			if (
				parameters != null
				&& parameters.ContainsKey("fromLeap")
				&& (bool)parameters["fromLeap"]
			)
			{
				_isRolling = true;

				Debug.Log("Entering Walking State with a Roll!");
			}
		}

		public override void Update()
		{
			base.Update();

			if (_isRolling)
			{
				_isRolling = false;
				// TODO: Play roll animation from animator reference in playerContext
				Debug.Log("Roll Complete, now walking normally.");
				return;
			}

			if (InputMoveDirection.magnitude == 0)
			{
				PlayerContext.StateMachine.TransitionTo(
					new IdleState(),
					new Dictionary<string, object> { { "previousSpeed", 0 } }
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
