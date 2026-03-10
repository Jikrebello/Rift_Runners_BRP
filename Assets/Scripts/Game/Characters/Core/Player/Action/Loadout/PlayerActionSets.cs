using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public static class PlayerActionSets
	{
		public static PlayerActionSet CreateDefault()
		{
			var set = new PlayerActionSet();

			ConfigureNeutralBank(set.NeutralBank);
			ConfigureAimBank(set.AimBank);
			ConfigureBlockBank(set.BlockBank);
			ConfigureSpellReadyBank(set.SpellReadyBank);

			return set;
		}

		private static void ConfigureNeutralBank(PlayerActionBank bank)
		{
			bank.LightAttackId = PlayerActionId.LightAttack;
			bank.HeavyAttackId = PlayerActionId.HeavyAttack;
			bank.RightActionId = PlayerActionId.ContextGrab;

			bank.SkillSlot1Id = PlayerActionId.Skill1;
			bank.SkillSlot2Id = PlayerActionId.Skill2;
			bank.SkillSlot3Id = PlayerActionId.Skill3;
		}

		private static void ConfigureAimBank(PlayerActionBank bank)
		{
			bank.LightAttackId = PlayerActionId.LightAttack;
			bank.HeavyAttackId = PlayerActionId.HeavyAttack;
			bank.RightActionId = PlayerActionId.FundamentalRangedPrimary;

			bank.SkillSlot1Id = PlayerActionId.Skill1;
			bank.SkillSlot2Id = PlayerActionId.Skill2;
			bank.SkillSlot3Id = PlayerActionId.Skill3;
		}

		private static void ConfigureBlockBank(PlayerActionBank bank)
		{
			bank.LightAttackId = PlayerActionId.LightAttack;
			bank.HeavyAttackId = PlayerActionId.HeavyAttack;
			bank.RightActionId = PlayerActionId.ContextGrab;

			bank.SkillSlot1Id = PlayerActionId.Skill1;
			bank.SkillSlot2Id = PlayerActionId.Skill2;
			bank.SkillSlot3Id = PlayerActionId.Skill3;
		}

		private static void ConfigureSpellReadyBank(PlayerActionBank bank)
		{
			bank.LightAttackId = PlayerActionId.LightAttack;
			bank.HeavyAttackId = PlayerActionId.HeavyAttack;
			bank.RightActionId = PlayerActionId.ContextGrab;

			bank.SkillSlot1Id = PlayerActionId.Skill1;
			bank.SkillSlot2Id = PlayerActionId.Skill2;
			bank.SkillSlot3Id = PlayerActionId.Skill3;
		}
	}
}
