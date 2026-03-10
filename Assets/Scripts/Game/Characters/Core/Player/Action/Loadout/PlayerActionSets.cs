using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public static class PlayerActionSets
	{
		public static PlayerActionSet CreateDefault()
		{
			return new PlayerActionSet
			{
				LightAttackId = PlayerActionId.LightAttack,
				HeavyAttackId = PlayerActionId.HeavyAttack,
				NeutralRightActionId = PlayerActionId.ContextGrab,
				SecondaryModeRightActionId = PlayerActionId.FundamentalRangedPrimary,
				SkillSlot1Id = PlayerActionId.Skill1,
				SkillSlot2Id = PlayerActionId.Skill2,
				SkillSlot3Id = PlayerActionId.Skill3,
			};
		}
	}
}
