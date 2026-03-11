namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public sealed class PlayerCombatSlotProfile
	{
		public PlayerCombatSlotKind SlotKind { get; set; } = PlayerCombatSlotKind.None;
		public PlayerModifierPostureEffect ModifierPostureEffect { get; set; } =
			PlayerModifierPostureEffect.None;
	}
}
