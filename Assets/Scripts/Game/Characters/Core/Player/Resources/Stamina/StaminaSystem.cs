using System;
using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Resolution;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina
{
	public sealed class StaminaSystem
	{
		private StaminaConfig _cfg;

		private enum IntentStaminaDecision
		{
			NotApplicable = 0,
			Approved = 1,
			Denied = 2,
		}

		public StaminaSystem(StaminaConfig cfg) => _cfg = cfg;

		public IReadOnlyList<IPlayerIntent> FilterAndApply(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents,
			float dt
		)
		{
			bool wasExhausted = model.IsExhausted;

			ApplyPassiveStamina(model, outputs, dt);

			bool enteredExhaustionThisTick = !wasExhausted && model.IsExhausted;

			if (!enteredExhaustionThisTick)
				UpdateExhaustionState(model, outputs, dt);

			if (model.IsExhausted)
				return FilterWhileExhausted(model, outputs, intents);

			var filtered = new List<IPlayerIntent>(intents.Count);

			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];
				var decision = EvaluateIntent(model, outputs, intent, out float cost);

				if (decision == IntentStaminaDecision.Denied)
					continue;

				if (decision == IntentStaminaDecision.Approved && cost > 0f)
					Spend(model, cost);

				filtered.Add(intent);
			}

			return filtered;
		}

		public ResolvedPlayerActionRequest? FilterAndApplyResolvedAction(
			PlayerModel model,
			PlayerOutputs outputs,
			ResolvedPlayerActionRequest? request
		)
		{
			if (!request.HasValue)
				return null;

			if (model.IsExhausted)
			{
				outputs.Debug.Warn("Stamina", "Denied action: player is exhausted.");
				return null;
			}

			var action = request.Value.Action;

			if (RequiresPositiveStamina(action.Id) && model.Stamina <= 0f)
			{
				outputs.Debug.Warn(
					"Stamina",
					$"Denied Action({action.Id}): stamina={model.Stamina:0.##}, requires stamina above zero."
				);
				return null;
			}

			var cost = action.Execution.StaminaCost;

			if (cost <= 0f)
				return request;

			if (model.Stamina < cost)
			{
				outputs.Debug.Warn(
					"Stamina",
					$"Denied Action({action.Id}): stamina={model.Stamina:0.##}, cost={cost:0.##}."
				);
				return null;
			}

			Spend(model, cost);
			return request;
		}

		public void SetConfig(StaminaConfig cfg) => _cfg = cfg;

		private static void CancelGlideIfExhausted(PlayerModel model, PlayerOutputs outputs)
		{
			if (!model.IsGliding)
				return;

			if (model.Stamina > 0f)
				return;

			model.IsGliding = false;
			outputs.Debug.Info("Stamina", "Glide cancelled: stamina exhausted.");
		}

		private static void CancelSprintIfExhausted(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.GroundedSubMode != PlayerGroundedSubMode.Sprinting)
				return;

			if (model.Stamina > 0f)
				return;

			model.GroundedSubMode = PlayerGroundedSubMode.Standing;
			outputs.Debug.Info("Stamina", "Sprint cancelled: stamina exhausted.");
		}

		private static float ClampStamina(float stamina, float maxStamina)
		{
			if (maxStamina <= 0f)
				return 0f;

			if (stamina < 0f)
				return 0f;

			if (stamina > maxStamina)
				return maxStamina;

			return stamina;
		}

		private static bool RequiresPositiveStamina(PlayerActionId actionId)
		{
			return actionId == PlayerActionId.LightAttack
				|| actionId == PlayerActionId.LightAttack2
				|| actionId == PlayerActionId.LightAttack3
				|| actionId == PlayerActionId.HeavyAttack;
		}

		private static void Spend(PlayerModel model, float cost)
		{
			if (cost <= 0f)
				return;

			model.Stamina -= cost;

			if (model.Stamina < 0f)
				model.Stamina = 0f;
		}

		private static IntentStaminaDecision TryHandleBeginGlideIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not BeginGlideIntent)
				return IntentStaminaDecision.NotApplicable;

			if (model.Stamina > 0f)
				return IntentStaminaDecision.Approved;

			outputs.Debug.Warn("Stamina", $"Denied BeginGlide: stamina={model.Stamina:0.##}.");
			return IntentStaminaDecision.Denied;
		}

		private void ApplyPassiveStamina(PlayerModel model, PlayerOutputs outputs, float dt)
		{
			float delta = CalculatePassiveDelta(model, dt);

			if (MathF.Abs(delta) > 0f)
			{
				model.Stamina = ClampStamina(model.Stamina + delta, model.MaxStamina);
			}

			CancelSprintIfExhausted(model, outputs);
			CancelGlideIfExhausted(model, outputs);

			if (!model.IsExhausted && model.Stamina <= 0f)
				EnterExhaustion(model, outputs);
		}

		private void BeginRecovery(PlayerModel model, PlayerOutputs outputs)
		{
			model.ExhaustionState = PlayerExhaustionState.Recovering;
			model.ExhaustionRecoveryRemaining = _cfg.Exhaustion.RecoveryDurationSeconds;
			model.MoveInput = Vector2.Zero;

			outputs.Debug.Info("Stamina", "Entered exhaustion recovery.");
		}

		private float CalculatePassiveDelta(PlayerModel model, float dt)
		{
			if (model.TraversalMode == PlayerTraversalMode.Grounded)
			{
				if (model.GroundedSubMode == PlayerGroundedSubMode.Sprinting)
					return -_cfg.Drain.SprintPerSecond * dt;

				if (model.GroundedSubMode == PlayerGroundedSubMode.Sliding)
					return _cfg.Regen.SlidingPerSecond * dt;

				return _cfg.Regen.GroundedPerSecond * dt;
			}

			if (model.IsGliding)
				return -_cfg.Drain.GlidePerSecond * dt;

			return 0f;
		}

		private void EnterExhaustion(PlayerModel model, PlayerOutputs outputs)
		{
			CancelSprintIfExhausted(model, outputs);
			CancelGlideIfExhausted(model, outputs);

			if (model.MoveInput.LengthSquared() > 0f)
			{
				model.ExhaustionState = PlayerExhaustionState.DrainedMoving;
				model.MoveInput *= _cfg.Exhaustion.DrainedMoveInitialMultiplier;

				outputs.Debug.Info("Stamina", "Entered exhaustion: drained movement.");
				return;
			}

			BeginRecovery(model, outputs);
		}

		private IReadOnlyList<IPlayerIntent> FilterWhileExhausted(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents
		)
		{
			var filtered = new List<IPlayerIntent>(0);

			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];

				if (intent is MoveIntent)
					continue;

				outputs.Debug.Info(
					"Stamina",
					$"Denied {intent.GetType().Name}: player is exhausted."
				);
			}

			return filtered;
		}

		private IntentStaminaDecision EvaluateIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			var decision = TryHandleSprintIntent(model, outputs, intent, out cost);
			if (decision != IntentStaminaDecision.NotApplicable)
				return decision;

			decision = TryHandleBeginGlideIntent(model, outputs, intent, out cost);
			if (decision != IntentStaminaDecision.NotApplicable)
				return decision;

			decision = TryHandleLeapIntent(model, outputs, intent, out cost);
			if (decision != IntentStaminaDecision.NotApplicable)
				return decision;

			decision = TryHandleKickOffIntent(model, outputs, intent, out cost);
			if (decision != IntentStaminaDecision.NotApplicable)
				return decision;

			decision = TryHandleDropIntent(model, outputs, intent, out cost);
			if (decision != IntentStaminaDecision.NotApplicable)
				return decision;

			decision = TryHandleUseSkillIntent(model, outputs, intent, out cost);
			if (decision != IntentStaminaDecision.NotApplicable)
				return decision;

			cost = 0f;
			return IntentStaminaDecision.NotApplicable;
		}

		private IntentStaminaDecision TryHandleDropIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not DropIntent)
				return IntentStaminaDecision.NotApplicable;

			cost = _cfg.Traversal.Drop;

			if (model.Stamina >= cost)
				return IntentStaminaDecision.Approved;

			outputs.Debug.Warn(
				"Stamina",
				$"Denied Drop: stamina={model.Stamina:0.##}, cost={cost:0.##}."
			);
			cost = 0f;
			return IntentStaminaDecision.Denied;
		}

		private IntentStaminaDecision TryHandleKickOffIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not KickOffIntent)
				return IntentStaminaDecision.NotApplicable;

			cost = _cfg.Traversal.KickOff;

			if (model.Stamina >= cost)
				return IntentStaminaDecision.Approved;

			outputs.Debug.Warn(
				"Stamina",
				$"Denied KickOff: stamina={model.Stamina:0.##}, cost={cost:0.##}."
			);
			cost = 0f;
			return IntentStaminaDecision.Denied;
		}

		private IntentStaminaDecision TryHandleLeapIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not LeapIntent)
				return IntentStaminaDecision.NotApplicable;

			cost = _cfg.Traversal.Leap;

			if (model.Stamina >= cost)
				return IntentStaminaDecision.Approved;

			outputs.Debug.Warn(
				"Stamina",
				$"Denied Leap: stamina={model.Stamina:0.##}, cost={cost:0.##}."
			);
			cost = 0f;
			return IntentStaminaDecision.Denied;
		}

		private IntentStaminaDecision TryHandleSprintIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not ToggleSprintIntent)
				return IntentStaminaDecision.NotApplicable;

			bool enteringSprint = model.GroundedSubMode != PlayerGroundedSubMode.Sprinting;
			if (!enteringSprint)
				return IntentStaminaDecision.Approved;

			if (model.Stamina >= _cfg.Traversal.MinStaminaToEnterSprint)
				return IntentStaminaDecision.Approved;

			outputs.Debug.Warn(
				"Stamina",
				$"Denied ToggleSprint: stamina={model.Stamina:0.##}, required={_cfg.Traversal.MinStaminaToEnterSprint:0.##}."
			);
			return IntentStaminaDecision.Denied;
		}

		private IntentStaminaDecision TryHandleUseSkillIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not UseSkillIntent skill)
				return IntentStaminaDecision.NotApplicable;

			cost = _cfg.GetSkillCost(skill.Bank, skill.Slot);

			if (cost <= 0f)
				return IntentStaminaDecision.Approved;

			if (model.Stamina >= cost)
				return IntentStaminaDecision.Approved;

			outputs.Debug.Warn(
				"Stamina",
				$"Denied UseSkill({skill.Bank},{skill.Slot}): stamina={model.Stamina:0.##}, cost={cost:0.##}."
			);
			cost = 0f;
			return IntentStaminaDecision.Denied;
		}

		private void UpdateDrainedMoving(PlayerModel model, PlayerOutputs outputs, float dt)
		{
			float decay = MathF.Max(0f, 1f - (_cfg.Exhaustion.DrainedMoveDecayPerSecond * dt));
			model.MoveInput *= decay;

			if (model.MoveInput.Length() <= _cfg.Exhaustion.StopMoveThreshold)
				BeginRecovery(model, outputs);
		}

		private void UpdateExhaustionState(PlayerModel model, PlayerOutputs outputs, float dt)
		{
			switch (model.ExhaustionState)
			{
				case PlayerExhaustionState.DrainedMoving:
					UpdateDrainedMoving(model, outputs, dt);
					break;

				case PlayerExhaustionState.Recovering:
					UpdateRecovering(model, outputs, dt);
					break;
			}
		}

		private void UpdateRecovering(PlayerModel model, PlayerOutputs outputs, float dt)
		{
			model.MoveInput = Vector2.Zero;
			model.ExhaustionRecoveryRemaining -= dt;

			if (model.ExhaustionRecoveryRemaining > 0f)
				return;

			if (model.Stamina <= 0f)
				return;

			model.ExhaustionState = PlayerExhaustionState.None;
			model.ExhaustionRecoveryRemaining = 0f;

			outputs.Debug.Info("Stamina", "Exhaustion recovery complete.");
		}
	}
}
