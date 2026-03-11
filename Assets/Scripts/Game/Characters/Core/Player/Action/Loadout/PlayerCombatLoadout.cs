namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public sealed class PlayerCombatLoadout
	{
		public PlayerCombatSlotProfile PrimarySlot { get; } = new();
		public PlayerCombatSlotProfile SecondarySlot { get; } = new();
		public PlayerActionSet ActionSet { get; } = new();
	}
}
