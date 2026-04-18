using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public static class PlayerActionSets
	{
		public static PlayerActionSet CreateDefault()
		{
			var set = new PlayerActionSet();

			ConfigureBaseBank(set.BaseBank);
			ConfigurePrimaryModifierBank(set.PrimaryModifierBank);
			ConfigureSecondaryModifierBank(set.SecondaryModifierBank);

			return set;
		}

		public static PlayerActionSet Clone(PlayerActionSet source)
		{
			var clone = new PlayerActionSet();
			CopyInto(source, clone);
			return clone;
		}

		public static void CopyInto(PlayerActionSet source, PlayerActionSet destination)
		{
			CopyBank(source.BaseBank, destination.BaseBank);
			CopyBank(source.PrimaryModifierBank, destination.PrimaryModifierBank);
			CopyBank(source.SecondaryModifierBank, destination.SecondaryModifierBank);
		}

		private static void ConfigureBaseBank(PlayerActionBank bank)
		{
			bank.PrimaryFaceActionId = PlayerActionId.LightAttack;
			bank.SecondaryFaceActionId = PlayerActionId.HeavyAttack;
			bank.TertiaryFaceActionId = PlayerActionId.ContextInteract;
			bank.RightActionId = PlayerActionId.ContextGrab;
		}

		private static void ConfigurePrimaryModifierBank(PlayerActionBank bank)
		{
			bank.PrimaryFaceActionId = PlayerActionId.SwordAdvanceSlash;
			bank.SecondaryFaceActionId = PlayerActionId.SwordSkillSecondary;
			bank.TertiaryFaceActionId = PlayerActionId.SwordSkillTertiary;
			bank.RightActionId = PlayerActionId.ContextGrab;
		}

		private static void ConfigureSecondaryModifierBank(PlayerActionBank bank)
		{
			bank.PrimaryFaceActionId = PlayerActionId.ShieldGuardBash;
			bank.SecondaryFaceActionId = PlayerActionId.ShieldSkillSecondary;
			bank.TertiaryFaceActionId = PlayerActionId.ShieldSkillTertiary;
			bank.RightActionId = PlayerActionId.FundamentalBlockPrimary;
		}

		private static void CopyBank(PlayerActionBank source, PlayerActionBank destination)
		{
			destination.PrimaryFaceActionId = source.PrimaryFaceActionId;
			destination.SecondaryFaceActionId = source.SecondaryFaceActionId;
			destination.TertiaryFaceActionId = source.TertiaryFaceActionId;
			destination.RightActionId = source.RightActionId;
		}
	}
}
