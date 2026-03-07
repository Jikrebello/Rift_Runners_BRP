using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.CombatMode
{
	public sealed class CombatModeSystem
	{
		public void HandleIntents(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents
		)
		{
			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];

				if (intent is ToggleWeaponStanceIntent)
				{
					ToggleStance(model);
					EmitCombatModeOutputs(model, outputs);
					continue;
				}

				if (intent is SecondaryModifierHeldIntent sec)
				{
					ApplySecondaryModifier(model, sec.IsHeld);
					EmitCombatModeOutputs(model, outputs);
				}
			}
		}

		private static void ApplySecondaryModifier(PlayerModel model, bool held)
		{
			// Rule:
			// - if held: auto-unholster and activate secondary mode (aim/block)
			// - if released: leave unholstered but exit secondary mode
			if (held)
			{
				if (model.CombatStance == PlayerCombatStance.Holstered)
					model.CombatStance = PlayerCombatStance.Unholstered;

				model.SecondaryMode = SecondaryModifierMode.Active;
				return;
			}

			model.SecondaryMode = SecondaryModifierMode.None;
		}

		private static void EmitCombatModeOutputs(PlayerModel model, PlayerOutputs outputs)
		{
			bool isHolstered = model.CombatStance == PlayerCombatStance.Holstered;
			bool secondaryActive = model.IsSecondaryModifierActive;

			outputs.Animation.AddBool(AnimBool.Holstered, isHolstered);
			outputs.Animation.AddBool(AnimBool.SecondaryModifierActive, secondaryActive);

			int upperBodyMode = secondaryActive
				? (int)model.EquippedUpperBodyMode
				: (int)UpperBodyMode.None;
			outputs.Animation.AddInt(AnimInt.UpperBodyMode, upperBodyMode);
		}

		private static void ToggleStance(PlayerModel model)
		{
			model.CombatStance =
				model.CombatStance == PlayerCombatStance.Holstered
					? PlayerCombatStance.Unholstered
					: PlayerCombatStance.Holstered;

			// If you holster, you cannot remain in modifier mode.
			if (model.CombatStance == PlayerCombatStance.Holstered)
				model.SecondaryMode = SecondaryModifierMode.None;
		}
	}
}
