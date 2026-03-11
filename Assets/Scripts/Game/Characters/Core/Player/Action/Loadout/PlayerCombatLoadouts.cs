namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public static class PlayerCombatLoadouts
	{
		public static PlayerCombatLoadout CreateDefault()
		{
			var loadout = new PlayerCombatLoadout();

			loadout.PrimarySlot.SlotKind = PlayerCombatSlotKind.Sword;
			loadout.PrimarySlot.ModifierPostureEffect = PlayerModifierPostureEffect.None;

			loadout.SecondarySlot.SlotKind = PlayerCombatSlotKind.Bow;
			loadout.SecondarySlot.ModifierPostureEffect = PlayerModifierPostureEffect.Aim;

			var defaultSet = PlayerActionSets.CreateDefault();

			CopyBank(defaultSet.BaseBank, loadout.ActionSet.BaseBank);
			CopyBank(defaultSet.PrimaryModifierBank, loadout.ActionSet.PrimaryModifierBank);
			CopyBank(defaultSet.SecondaryModifierBank, loadout.ActionSet.SecondaryModifierBank);
			CopyBank(defaultSet.DualModifierBank, loadout.ActionSet.DualModifierBank);

			return loadout;
		}

		private static void CopyBank(PlayerActionBank source, PlayerActionBank destination)
		{
			destination.LightAttackId = source.LightAttackId;
			destination.HeavyAttackId = source.HeavyAttackId;
			destination.RightActionId = source.RightActionId;
			destination.SkillSlot1Id = source.SkillSlot1Id;
			destination.SkillSlot2Id = source.SkillSlot2Id;
			destination.SkillSlot3Id = source.SkillSlot3Id;
		}
	}
}
