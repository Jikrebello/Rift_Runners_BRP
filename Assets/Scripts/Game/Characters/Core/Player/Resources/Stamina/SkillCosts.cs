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
			float primarySlot1,
			float primarySlot2,
			float primarySlot3,
			float secondarySlot1,
			float secondarySlot2,
			float secondarySlot3
		)
		{
			PrimarySlot1 = primarySlot1;
			PrimarySlot2 = primarySlot2;
			PrimarySlot3 = primarySlot3;
			SecondarySlot1 = secondarySlot1;
			SecondarySlot2 = secondarySlot2;
			SecondarySlot3 = secondarySlot3;
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
