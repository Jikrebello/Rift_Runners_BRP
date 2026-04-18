namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public static class PlayerCombatLoadouts
	{
		public const string DefaultLoadoutId = "Default";

		public static PlayerCombatLoadout CreateDefault()
		{
			var loadout = new PlayerCombatLoadout();

			loadout.PrimarySlot.SlotKind = PlayerCombatSlotKind.Sword;
			loadout.PrimarySlot.ModifierPostureEffect = PlayerModifierPostureEffect.None;

			loadout.SecondarySlot.SlotKind = PlayerCombatSlotKind.Shield;
			loadout.SecondarySlot.ModifierPostureEffect = PlayerModifierPostureEffect.Block;

			var defaultSet = PlayerActionSets.CreateDefault();
			PlayerActionSets.CopyInto(defaultSet, loadout.ActionSet);

			return loadout;
		}

		public static PlayerCombatLoadoutCatalog CreateDefaultCatalog()
		{
			return new PlayerCombatLoadoutCatalog(
				DefaultLoadoutId,
				new System.Collections.Generic.Dictionary<string, PlayerCombatLoadout>
				{
					[DefaultLoadoutId] = CreateDefault(),
				}
			);
		}

		public static PlayerCombatLoadout Clone(PlayerCombatLoadout source)
		{
			var clone = new PlayerCombatLoadout();
			CopyInto(source, clone);
			return clone;
		}

		public static void CopyInto(PlayerCombatLoadout source, PlayerCombatLoadout destination)
		{
			CopySlot(source.PrimarySlot, destination.PrimarySlot);
			CopySlot(source.SecondarySlot, destination.SecondarySlot);
			PlayerActionSets.CopyInto(source.ActionSet, destination.ActionSet);
		}

		private static void CopySlot(
			PlayerCombatSlotProfile source,
			PlayerCombatSlotProfile destination
		)
		{
			destination.SlotKind = source.SlotKind;
			destination.ModifierPostureEffect = source.ModifierPostureEffect;
		}
	}
}
