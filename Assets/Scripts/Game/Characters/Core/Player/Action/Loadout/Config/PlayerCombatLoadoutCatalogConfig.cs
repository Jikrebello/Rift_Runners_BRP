#nullable disable
using System.Collections.Generic;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout.Config
{
	public sealed class PlayerCombatLoadoutCatalogConfig
	{
		public string DefaultLoadoutId { get; set; } = string.Empty;
		public List<PlayerCombatLoadoutConfig> Loadouts { get; set; } =
			new List<PlayerCombatLoadoutConfig>();
	}

	public sealed class PlayerCombatLoadoutConfig
	{
		public string Id { get; set; } = string.Empty;
		public PlayerCombatSlotProfileConfig PrimarySlot { get; set; } =
			new PlayerCombatSlotProfileConfig();
		public PlayerCombatSlotProfileConfig SecondarySlot { get; set; } =
			new PlayerCombatSlotProfileConfig();
		public PlayerActionSetConfig ActionSet { get; set; } = new PlayerActionSetConfig();
	}

	public sealed class PlayerCombatSlotProfileConfig
	{
		public string SlotKind { get; set; } = string.Empty;
		public string ModifierPostureEffect { get; set; } = string.Empty;
	}

	public sealed class PlayerActionSetConfig
	{
		public PlayerActionBankConfig BaseBank { get; set; } = new PlayerActionBankConfig();
		public PlayerActionBankConfig PrimaryModifierBank { get; set; } =
			new PlayerActionBankConfig();
		public PlayerActionBankConfig SecondaryModifierBank { get; set; } =
			new PlayerActionBankConfig();
	}

	public sealed class PlayerActionBankConfig
	{
		public string PrimaryFaceActionId { get; set; } = string.Empty;
		public string SecondaryFaceActionId { get; set; } = string.Empty;
		public string TertiaryFaceActionId { get; set; } = string.Empty;
		public string RightActionId { get; set; } = string.Empty;
	}
}
