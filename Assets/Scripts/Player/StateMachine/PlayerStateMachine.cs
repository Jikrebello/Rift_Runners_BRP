using System.Collections.Generic;
using Assets.Scripts.Player.StateMachine.States;

namespace Assets.Scripts.Player.StateMachine
{
	public class PlayerStateMachine
	{
		private IPlayerState _currentState;
		private readonly PlayerContext _playerContext;

		public PlayerStateMachine(PlayerContext playerContext)
		{
			_playerContext = playerContext;
		}

		public void Update()
		{
			_currentState?.Update();
		}

		public void FixedUpdate()
		{
			_currentState?.FixedUpdate();
		}

		public void TransitionTo(
			IPlayerState newState,
			Dictionary<string, object> transitionParameters = null
		)
		{
			_currentState?.Exit();
			_currentState = newState;
			_currentState.PlayerContext = _playerContext;
			_currentState.Enter(transitionParameters);
		}
	}
}
