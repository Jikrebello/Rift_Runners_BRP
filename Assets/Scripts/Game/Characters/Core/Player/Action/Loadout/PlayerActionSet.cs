namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public sealed class PlayerActionSet
	{
		public PlayerActionBank BaseBank { get; } = new();
		public PlayerActionBank PrimaryModifierBank { get; } = new();
		public PlayerActionBank SecondaryModifierBank { get; } = new();
		public PlayerActionBank DualModifierBank { get; } = new();

		public PlayerActionBank GetBank(PlayerActionBankSelector selector)
		{
			return selector switch
			{
				PlayerActionBankSelector.PrimaryModifier => PrimaryModifierBank,
				PlayerActionBankSelector.SecondaryModifier => SecondaryModifierBank,
				PlayerActionBankSelector.DualModifier => DualModifierBank,
				_ => BaseBank,
			};
		}
	}
}
