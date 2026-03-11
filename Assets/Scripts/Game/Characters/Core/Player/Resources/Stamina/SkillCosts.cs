using Assets.Scripts.Game.Characters.Core.Player.Intent;

namespace Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina
{
	public readonly struct SkillCosts
	{
		public readonly float PrimarySlot1;
		public readonly float PrimarySlot2;
		public readonly float PrimarySlot3;
		public readonly float SecondarySlot1;
		public readonly float SecondarySlot2;
		public readonly float SecondarySlot3;

		public SkillCosts(
			float primarySkillSlot1Cost,
			float primarySkillSlot2Cost,
			float primarySkillSlot3Cost,
			float secondarySkillSlot1Cost,
			float secondarySkillSlot2Cost,
			float secondarySkillSlot3Cost
		)
		{
			PrimarySlot1 = primarySkillSlot1Cost;
			PrimarySlot2 = primarySkillSlot2Cost;
			PrimarySlot3 = primarySkillSlot3Cost;
			SecondarySlot1 = secondarySkillSlot1Cost;
			SecondarySlot2 = secondarySkillSlot2Cost;
			SecondarySlot3 = secondarySkillSlot3Cost;
		}

		public float GetCost(SkillBank bank, int slot)
		{
			return bank switch
			{
				SkillBank.Primary => slot switch
				{
					1 => PrimarySlot1,
					2 => PrimarySlot2,
					3 => PrimarySlot3,
					_ => 0f,
				},
				SkillBank.Secondary => slot switch
				{
					1 => SecondarySlot1,
					2 => SecondarySlot2,
					3 => SecondarySlot3,
					_ => 0f,
				},
				_ => 0f,
			};
		}
	}
}
