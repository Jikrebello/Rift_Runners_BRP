namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public static class PlayerCombatProfiles
	{
		public static PlayerCombatProfile CreateDefault()
		{
			var profile = new PlayerCombatProfile();
			var defaultSet = PlayerActionSets.CreateDefault();

			CopyBank(defaultSet.NeutralBank, profile.ActionSet.NeutralBank);
			CopyBank(defaultSet.AimBank, profile.ActionSet.AimBank);
			CopyBank(defaultSet.BlockBank, profile.ActionSet.BlockBank);
			CopyBank(defaultSet.SpellReadyBank, profile.ActionSet.SpellReadyBank);

			return profile;
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
