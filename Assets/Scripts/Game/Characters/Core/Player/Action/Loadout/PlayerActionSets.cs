using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public static class PlayerActionSets
	{
		public static PlayerActionSet CreateDefault()
		{
			var set = new PlayerActionSet();

			// Neutral bank
			set.NeutralBank.LightAttackId = PlayerActionId.LightAttack;
			set.NeutralBank.HeavyAttackId = PlayerActionId.HeavyAttack;
			set.NeutralBank.RightActionId = PlayerActionId.ContextGrab;

			// Aim bank
			set.AimBank.LightAttackId = PlayerActionId.LightAttack;
			set.AimBank.HeavyAttackId = PlayerActionId.HeavyAttack;
			set.AimBank.RightActionId = PlayerActionId.FundamentalRangedPrimary;

			// Block bank
			set.BlockBank.LightAttackId = PlayerActionId.LightAttack;
			set.BlockBank.HeavyAttackId = PlayerActionId.HeavyAttack;
			set.BlockBank.RightActionId = PlayerActionId.ContextGrab;

			// Spell-ready bank
			set.SpellReadyBank.LightAttackId = PlayerActionId.LightAttack;
			set.SpellReadyBank.HeavyAttackId = PlayerActionId.HeavyAttack;
			set.SpellReadyBank.RightActionId = PlayerActionId.ContextGrab;

			// Global skill slots for now
			set.SkillSlot1Id = PlayerActionId.Skill1;
			set.SkillSlot2Id = PlayerActionId.Skill2;
			set.SkillSlot3Id = PlayerActionId.Skill3;

			return set;
		}
	}
}
