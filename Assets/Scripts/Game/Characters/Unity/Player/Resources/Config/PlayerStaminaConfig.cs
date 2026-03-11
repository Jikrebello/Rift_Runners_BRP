using System;
using Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina;

namespace Assets.Scripts.Game.Characters.Unity.Player.Resources.Config
{
	[Serializable]
	public sealed class PlayerStaminaConfig
	{
		public float MaxStamina = 100f;
		public float GroundedRegenPerSecond = 18f;
		public float SlidingRegenPerSecond = 28f;
		public float SprintDrainPerSecond = 14f;
		public float GlideDrainPerSecond = 22f;
		public float LeapCost = 20f;
		public float KickOffCost = 10f;
		public float DropCost = 8f;
		public float MinStaminaToEnterSprint = 12f;

		public float PrimarySkillSlot1Cost = 0f;
		public float PrimarySkillSlot2Cost = 0f;
		public float PrimarySkillSlot3Cost = 0f;
		public float SecondarySkillSlot1Cost = 0f;
		public float SecondarySkillSlot2Cost = 0f;
		public float SecondarySkillSlot3Cost = 0f;
	}

	public static class PlayerStaminaConfigMapper
	{
		public static StaminaConfig ToStaminaConfig(PlayerStaminaConfig cfg)
		{
			return new StaminaConfig(
				cfg.MaxStamina,
				new RegenRates(cfg.GroundedRegenPerSecond, cfg.SlidingRegenPerSecond),
				new DrainRates(cfg.SprintDrainPerSecond, cfg.GlideDrainPerSecond),
				new TraversalCosts(
					cfg.LeapCost,
					cfg.KickOffCost,
					cfg.DropCost,
					cfg.MinStaminaToEnterSprint
				),
				new SkillCosts(
					cfg.PrimarySkillSlot1Cost,
					cfg.PrimarySkillSlot2Cost,
					cfg.PrimarySkillSlot3Cost,
					cfg.SecondarySkillSlot1Cost,
					cfg.SecondarySkillSlot2Cost,
					cfg.SecondarySkillSlot3Cost
				),
				exhaustion: new ExhaustionSettings(
					drainedMoveInitialMultiplier: 0.35f,
					drainedMoveDecayPerSecond: 4f,
					recoveryDurationSeconds: 1.0f,
					stopMoveThreshold: 0.05f
				)
			);
		}
	}

	public static class PlayerStaminaConfigPaths
	{
		public const string PlayerStaminaConfigPath = "Configs/Player/player_stamina_config.json";
	}
}
