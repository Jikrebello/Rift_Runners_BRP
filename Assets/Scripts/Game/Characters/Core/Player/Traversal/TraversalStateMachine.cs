using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Traversal
{
	public sealed class TraversalStateMachine
	{
		private ITraversalState _state;

		public void HandleIntents(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents
		) => _state?.HandleIntents(model, outputs, intents);

		public void Tick(
			PlayerModel model,
			PlayerOutputs outputs,
			in PlayerWorldSnapshot world,
			float dt
		) => _state?.Tick(model, outputs, world, dt);

		public void TransitionTo(ITraversalState next, PlayerModel model, PlayerOutputs outputs)
		{
			_state?.Exit(model, outputs);
			_state = next;
			_state.Enter(model, outputs);
		}
	}
}
