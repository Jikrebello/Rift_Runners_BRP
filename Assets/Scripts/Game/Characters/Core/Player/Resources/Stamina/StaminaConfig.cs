using Assets.Scripts.Game.Characters.Core.Player.Intent;

namespace Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina
{
	public readonly struct StaminaConfig
	{
		public readonly float MaxStamina;
		public readonly RegenRates Regen;
		public readonly DrainRates Drain;
		public readonly TraversalCosts Traversal;
		public readonly SkillCosts Skills;

		public StaminaConfig(
			float maxStamina,
			RegenRates regen,
			DrainRates drain,
			TraversalCosts traversal,
			SkillCosts skills
		)
		{
			MaxStamina = maxStamina;
			Regen = regen;
			Drain = drain;
			Traversal = traversal;
			Skills = skills;
		}

		public float GetSkillCost(SkillBank bank, int slot)
		{
			return Skills.GetCost(bank, slot);
		}
	}
}
