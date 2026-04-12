using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public static class PlayerActionSets
	{
		public static PlayerActionSet CreateDefault()
		{
			var set = new PlayerActionSet();

			ConfigureBaseBank(set.BaseBank);
			ConfigurePrimaryModifierBank(set.PrimaryModifierBank);
			ConfigureSecondaryModifierBank(set.SecondaryModifierBank);
			ConfigureDualModifierBank(set.DualModifierBank);

			return set;
		}

		public static PlayerActionSet Clone(PlayerActionSet source)
		{
			var clone = new PlayerActionSet();
			CopyInto(source, clone);
			return clone;
		}

		public static void CopyInto(PlayerActionSet source, PlayerActionSet destination)
		{
			CopyBank(source.BaseBank, destination.BaseBank);
			CopyBank(source.PrimaryModifierBank, destination.PrimaryModifierBank);
			CopyBank(source.SecondaryModifierBank, destination.SecondaryModifierBank);
			CopyBank(source.DualModifierBank, destination.DualModifierBank);
		}

		private static void ConfigureBaseBank(PlayerActionBank bank)
		{
			bank.LightAttackId = PlayerActionId.LightAttack;
			bank.HeavyAttackId = PlayerActionId.HeavyAttack;
			bank.RightActionId = PlayerActionId.ContextGrab;

			bank.SkillSlot1Id = PlayerActionId.None;
			bank.SkillSlot2Id = PlayerActionId.None;
			bank.SkillSlot3Id = PlayerActionId.None;
		}

		private static void ConfigurePrimaryModifierBank(PlayerActionBank bank)
		{
			bank.LightAttackId = PlayerActionId.Skill1;
			bank.HeavyAttackId = PlayerActionId.Skill2;
			bank.RightActionId = PlayerActionId.Skill3;

			bank.SkillSlot1Id = PlayerActionId.Skill1;
			bank.SkillSlot2Id = PlayerActionId.Skill2;
			bank.SkillSlot3Id = PlayerActionId.Skill3;
		}

		private static void ConfigureSecondaryModifierBank(PlayerActionBank bank)
		{
			bank.LightAttackId = PlayerActionId.Skill1;
			bank.HeavyAttackId = PlayerActionId.Skill2;
			bank.RightActionId = PlayerActionId.FundamentalRangedPrimary;

			bank.SkillSlot1Id = PlayerActionId.Skill1;
			bank.SkillSlot2Id = PlayerActionId.Skill2;
			bank.SkillSlot3Id = PlayerActionId.Skill3;
		}

		private static void ConfigureDualModifierBank(PlayerActionBank bank)
		{
			bank.LightAttackId = PlayerActionId.Skill1;
			bank.HeavyAttackId = PlayerActionId.Skill2;
			bank.RightActionId = PlayerActionId.Skill3;

			bank.SkillSlot1Id = PlayerActionId.Skill1;
			bank.SkillSlot2Id = PlayerActionId.Skill2;
			bank.SkillSlot3Id = PlayerActionId.Skill3;
		}

		private static void CopyBank(PlayerActionBank source, PlayerActionBank destination)
		{
			destination.LightAttackId = source.LightAttackId;
			destination.HeavyAttackId = source.HeavyAttackId;
			destination.ContextInteractId = source.ContextInteractId;
			destination.RightActionId = source.RightActionId;
			destination.SkillSlot1Id = source.SkillSlot1Id;
			destination.SkillSlot2Id = source.SkillSlot2Id;
			destination.SkillSlot3Id = source.SkillSlot3Id;
		}
	}
}
