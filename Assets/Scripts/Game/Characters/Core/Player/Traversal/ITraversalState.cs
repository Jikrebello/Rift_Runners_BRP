using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Traversal
{
	public interface ITraversalState
	{
		void Enter(PlayerModel model, PlayerOutputs outputs);

		void Exit(PlayerModel model, PlayerOutputs outputs);

		void HandleIntents(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents
		);

		void Tick(PlayerModel model, PlayerOutputs outputs, in PlayerWorldSnapshot world, float dt);
	}
}
