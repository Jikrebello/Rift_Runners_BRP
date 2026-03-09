using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina
{
	public sealed class StaminaSystem
	{
		private StaminaConfig _cfg;

		public StaminaSystem(StaminaConfig cfg) => _cfg = cfg;

		public IReadOnlyList<IPlayerIntent> FilterAndApply(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents,
			float dt
		)
		{
			ApplyPassiveStamina(model, outputs, dt);

			var filtered = new List<IPlayerIntent>(intents.Count);

			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];

				if (ShouldDeny(model, outputs, intent, out float cost))
					continue;

				if (cost > 0f)
					Spend(model, cost);

				filtered.Add(intent);
			}

			return filtered;
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

		private static void Spend(PlayerModel model, float cost)
		{
			if (cost <= 0f)
				return;

			model.Stamina -= cost;

			if (model.Stamina < 0f)
				model.Stamina = 0f;
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

		private bool ShouldDeny(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			if (TryHandleSprintIntent(model, outputs, intent, out cost))
				return true;

			if (TryHandleBeginGlideIntent(model, outputs, intent, out cost))
				return true;

			if (TryHandleLeapIntent(model, outputs, intent, out cost))
				return true;

			if (TryHandleKickOffIntent(model, outputs, intent, out cost))
				return true;

			if (TryHandleUseSkillIntent(model, outputs, intent, out cost))
				return true;

			cost = 0f;
			return false;
		}

		private static bool TryHandleBeginGlideIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not BeginGlideIntent)
				return false;

			if (model.Stamina > 0f)
				return false;

			outputs.Debug.Warn("Stamina", $"Denied BeginGlide: stamina={model.Stamina:0.##}.");
			return true;
		}

		private bool TryHandleKickOffIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not KickOffIntent)
				return false;

			cost = _cfg.Traversal.KickOff;

			if (model.Stamina >= cost)
				return false;

			outputs.Debug.Warn(
				"Stamina",
				$"Denied KickOff: stamina={model.Stamina:0.##}, cost={cost:0.##}."
			);
			cost = 0f;
			return true;
		}

		private bool TryHandleLeapIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not LeapIntent)
				return false;

			cost = _cfg.Traversal.Leap;

			if (model.Stamina >= cost)
				return false;

			outputs.Debug.Warn(
				"Stamina",
				$"Denied Leap: stamina={model.Stamina:0.##}, cost={cost:0.##}."
			);
			cost = 0f;
			return true;
		}

		private bool TryHandleSprintIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not ToggleSprintIntent)
				return false;

			bool enteringSprint = model.GroundedSubMode != PlayerGroundedSubMode.Sprinting;
			if (!enteringSprint)
				return false;

			if (model.Stamina >= _cfg.Traversal.MinStaminaToEnterSprint)
				return false;

			outputs.Debug.Warn(
				"Stamina",
				$"Denied ToggleSprint: stamina={model.Stamina:0.##}, required={_cfg.Traversal.MinStaminaToEnterSprint:0.##}."
			);
			return true;
		}

		private bool TryHandleUseSkillIntent(
			PlayerModel model,
			PlayerOutputs outputs,
			IPlayerIntent intent,
			out float cost
		)
		{
			cost = 0f;

			if (intent is not UseSkillIntent skill)
				return false;

			cost = _cfg.GetSkillCost(skill.Bank, skill.Slot);

			if (cost <= 0f)
				return false;

			if (model.Stamina >= cost)
				return false;

			outputs.Debug.Warn(
				"Stamina",
				$"Denied UseSkill({skill.Bank},{skill.Slot}): stamina={model.Stamina:0.##}, cost={cost:0.##}."
			);
			cost = 0f;
			return true;
		}
	}
}
