using System;
using Assets.Scripts.Game.Characters.Core.Player.Action;
using Assets.Scripts.Game.Characters.Core.Player.Action.Resolution;
using Assets.Scripts.Game.Characters.Core.Player.CombatMode;
using Assets.Scripts.Game.Characters.Core.Player.Input;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina;
using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s;

namespace Assets.Scripts.Game.Characters.Core.Player
{
	public sealed class PlayerPiece
	{
		private readonly ActionSystem _actionSystem = new();
		private readonly PlayerActionResolver _actionResolver = new();
		private readonly AirborneState _airborne = new();
		private readonly CombatModeSystem _combatModeSystem = new();
		private readonly GroundedState _grounded = new();
		private readonly PlayerIntentResolver _intentResolver;
		private readonly PlayerModel _model = new();
		private readonly PlayerOutputs _outputs = new();
		private readonly StaminaSystem _staminaSystem;
		private readonly TraversalActionIntentSynthesizer _traversalActionIntentSynthesizer;
		private readonly TraversalStateMachine _traversal = new();
		private readonly TraversalCoordinator _traversalCoordinator;

		private readonly SlidingState _sliding;

		public PlayerPiece(SlidingStateConfig slidingCfg, StaminaConfig staminaCfg)
		{
			_intentResolver = new PlayerIntentResolver(new PlayerIntentConfig());

			_sliding = new SlidingState(slidingCfg);
			_traversalActionIntentSynthesizer = new TraversalActionIntentSynthesizer(slidingCfg);
			_staminaSystem = new StaminaSystem(staminaCfg);

			_model.MaxStamina = staminaCfg.MaxStamina;
			_model.Stamina = staminaCfg.MaxStamina;

			_traversalCoordinator = new TraversalCoordinator(
				_traversal,
				_grounded,
				_sliding,
				_airborne
			);

			_traversal.TransitionTo(_grounded, _model, _outputs);
		}

		public void SetSlidingConfig(SlidingStateConfig cfg)
		{
			_sliding.SetConfig(cfg);
			_traversalActionIntentSynthesizer.SetSlidingConfig(cfg);
		}

		public void SetStaminaConfig(StaminaConfig cfg)
		{
			_staminaSystem.SetConfig(cfg);

			if (_model.MaxStamina <= 0f && _model.Stamina <= 0f)
			{
				_model.MaxStamina = cfg.MaxStamina;
				_model.Stamina = cfg.MaxStamina;
				return;
			}

			_model.MaxStamina = cfg.MaxStamina;
			_model.Stamina = MathF.Min(_model.Stamina, _model.MaxStamina);
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
			intents = _traversalActionIntentSynthesizer.Synthesize(_model, intents, dt);

			ResolvedPlayerActionRequest? actionRequest = null;
			if (_actionResolver.TryResolve(_model, intents, out var resolvedAction))
				actionRequest = resolvedAction;

			intents = _staminaSystem.FilterAndApply(_model, _outputs, intents, dt);
			actionRequest = _staminaSystem.FilterAndApplyResolvedAction(
				_model,
				_outputs,
				actionRequest
			);

			_combatModeSystem.HandleIntents(_model, _outputs, intents);
			_actionSystem.HandleResolvedAction(_model, _outputs, actionRequest);
			_actionSystem.Tick(_model, _outputs, dt);
			_traversal.HandleIntents(_model, _outputs, intents);

			_traversalCoordinator.ApplyTransitions(_model, _outputs, world, intents);
			_traversal.Tick(_model, _outputs, world, dt);

			return _outputs;
		}
	}
}
