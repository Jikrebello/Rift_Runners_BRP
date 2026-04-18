using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public sealed class PlayerActionBank
	{
		public PlayerActionId PrimaryFaceActionId { get; set; } = PlayerActionId.None;
		public PlayerActionId SecondaryFaceActionId { get; set; } = PlayerActionId.None;
		public PlayerActionId TertiaryFaceActionId { get; set; } = PlayerActionId.None;
		public PlayerActionId RightActionId { get; set; } = PlayerActionId.None;
	}
}
