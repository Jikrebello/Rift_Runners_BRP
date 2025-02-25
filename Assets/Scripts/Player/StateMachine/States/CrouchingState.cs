using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States
{
	class CrouchingState : GroundedState
	{
		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);
		}

		public override void Update()
		{
			base.Update();

			CurrentSubState = GroundedSubStates.Crouching;
			Debug.Log($"IdleState: Player Input Direction {CurrentSubState}");
			Debug.Log($"IdleState: Player Input Direction {InputMoveDirection}");
		}

		public override void FixedUpdate()
		{
			throw new NotImplementedException();
		}

		public override void Exit()
		{
			base.Exit();
		}
	}
}
