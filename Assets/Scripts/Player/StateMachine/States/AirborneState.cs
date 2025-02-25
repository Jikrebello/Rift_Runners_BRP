using System;
using System.Collections.Generic;

namespace Assets.Scripts.Player.StateMachine.States
{
	public class AirborneState : IPlayerState
	{
		public PlayerContext PlayerContext { get; set; }

		protected AirborneSubStates CurrentSubState { get; set; }

		public void Enter(Dictionary<string, object> parameters)
		{
			throw new NotImplementedException();
		}

		public void Update()
		{
			throw new NotImplementedException();
		}

		public void FixedUpdate()
		{
			throw new NotImplementedException();
		}

		public void Exit()
		{
			throw new NotImplementedException();
		}
	}
}
