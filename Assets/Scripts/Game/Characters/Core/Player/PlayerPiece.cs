using Assets.Scripts.Game.Characters.Core.Player.Action;
using Assets.Scripts.Game.Characters.Core.Player.CombatMode;
using Assets.Scripts.Game.Characters.Core.Player.Input;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Traversal;

namespace Assets.Scripts.Game.Characters.Core.Player
{
	public sealed class PlayerPiece
	{
		private readonly ActionSystem _actionSystem = new();
		private readonly AirborneState _airborne = new();
		private readonly CombatModeSystem _combatModeSystem = new();
		private readonly GroundedState _grounded = new();
		private readonly PlayerIntentResolver _intentResolver;
		private readonly PlayerModel _model = new();
		private readonly PlayerOutputs _outputs = new();
		private readonly SlidingState _sliding = new();
		private readonly TraversalStateMachine _traversal = new();
		private readonly TraversalCoordinator _traversalCoordinator;

		public PlayerPiece()
		{
			_intentResolver = new PlayerIntentResolver(new PlayerIntentConfig());
			_traversalCoordinator = new TraversalCoordinator(
				_traversal,
				_grounded,
				_sliding,
				_airborne
			);
			_traversal.TransitionTo(_grounded, _model, _outputs);
		}

		public PlayerOutputs Tick(
			in PlayerInputSnapshot input,
			in PlayerWorldSnapshot world,
			float dt
		)
		{
			_outputs.Clear();

			var rawIntents = _intentResolver.Resolve(input);
			var intents = PlayerIntentArbiter.Arbitrate(_model, world, rawIntents);

			_combatModeSystem.HandleIntents(_model, _outputs, intents);
			_actionSystem.HandleIntents(_model, _outputs, intents);
			_traversal.HandleIntents(_model, _outputs, intents);

			_traversalCoordinator.ApplyTransitions(_model, _outputs, world);

			_traversal.Tick(_model, _outputs, world, dt);

			return _outputs;
		}
	}
}
