using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public sealed class PlayerActionSet
	{
		public PlayerActionId LightAttackId { get; set; } = PlayerActionId.LightAttack;
		public PlayerActionId HeavyAttackId { get; set; } = PlayerActionId.HeavyAttack;

		public PlayerActionId NeutralRightActionId { get; set; } = PlayerActionId.ContextGrab;
		public PlayerActionId SecondaryModeRightActionId { get; set; } =
			PlayerActionId.FundamentalRangedPrimary;

		public PlayerActionId SkillSlot1Id { get; set; } = PlayerActionId.Skill1;
		public PlayerActionId SkillSlot2Id { get; set; } = PlayerActionId.Skill2;
		public PlayerActionId SkillSlot3Id { get; set; } = PlayerActionId.Skill3;
	}
}
