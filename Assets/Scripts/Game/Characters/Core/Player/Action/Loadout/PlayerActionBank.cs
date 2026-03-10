using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public sealed class PlayerActionBank
	{
		public PlayerActionId LightAttackId { get; set; } = PlayerActionId.None;
		public PlayerActionId HeavyAttackId { get; set; } = PlayerActionId.None;
		public PlayerActionId RightActionId { get; set; } = PlayerActionId.None;

		public PlayerActionId SkillSlot1Id { get; set; } = PlayerActionId.None;
		public PlayerActionId SkillSlot2Id { get; set; } = PlayerActionId.None;
		public PlayerActionId SkillSlot3Id { get; set; } = PlayerActionId.None;
	}
}
