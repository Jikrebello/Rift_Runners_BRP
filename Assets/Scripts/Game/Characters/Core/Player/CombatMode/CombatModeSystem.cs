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
			if (held)
			{
				if (model.CombatStance == PlayerCombatStance.Holstered)
					model.CombatStance = PlayerCombatStance.Unholstered;

				model.SecondaryMode = SecondaryModifierMode.Active;
				model.CombatPosture = ResolvePosture(model.EquippedUpperBodyMode);
				return;
			}

			model.SecondaryMode = SecondaryModifierMode.None;
			model.CombatPosture = PlayerCombatPosture.None;
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

		private static PlayerCombatPosture ResolvePosture(UpperBodyMode upperBodyMode)
		{
			return upperBodyMode switch
			{
				UpperBodyMode.Aim => PlayerCombatPosture.Aim,
				UpperBodyMode.Block => PlayerCombatPosture.Block,
				UpperBodyMode.SpellReady => PlayerCombatPosture.SpellReady,
				_ => PlayerCombatPosture.None,
			};
		}

		private static void ToggleStance(PlayerModel model)
		{
			model.CombatStance =
				model.CombatStance == PlayerCombatStance.Holstered
					? PlayerCombatStance.Unholstered
					: PlayerCombatStance.Holstered;

			if (model.CombatStance == PlayerCombatStance.Holstered)
			{
				model.SecondaryMode = SecondaryModifierMode.None;
				model.CombatPosture = PlayerCombatPosture.None;
			}
		}
	}
}
