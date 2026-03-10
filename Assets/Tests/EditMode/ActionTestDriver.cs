using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action;
using Assets.Scripts.Game.Characters.Core.Player.Action.Resolution;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Tests.EditMode
{
	internal sealed class ActionTestDriver
	{
		private readonly PlayerActionResolver _resolver = new();
		private readonly ActionSystem _system = new();

		public void Step(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents,
			float dt
		)
		{
			ResolvedPlayerActionRequest? request = null;

			if (_resolver.TryResolve(model, intents, out var resolved))
				request = resolved;

			_system.HandleResolvedAction(model, outputs, request);
			_system.Tick(model, outputs, dt);
		}
	}
}
