using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Loadout;
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

				if (intent is PrimaryModifierHeldIntent primary)
				{
					ApplyPrimaryModifier(model, primary.IsHeld);
					EmitCombatModeOutputs(model, outputs);
					continue;
				}

				if (intent is SecondaryModifierHeldIntent secondary)
				{
					ApplySecondaryModifier(model, secondary.IsHeld);
					EmitCombatModeOutputs(model, outputs);
				}
			}
		}

		private static void ApplyPrimaryModifier(PlayerModel model, bool held)
		{
			model.PrimaryMode = held ? PrimaryModifierMode.Active : PrimaryModifierMode.None;
		}

		private static void ApplySecondaryModifier(PlayerModel model, bool held)
		{
			if (held)
			{
				if (model.CombatStance == PlayerCombatStance.Holstered)
					model.CombatStance = PlayerCombatStance.Unholstered;

				model.SecondaryMode = SecondaryModifierMode.Active;
				model.CombatPosture = ResolveSecondaryPosture(model);
				model.EquippedUpperBodyMode = ResolveUpperBodyMode(model.CombatPosture);
				return;
			}

			model.SecondaryMode = SecondaryModifierMode.None;
			model.CombatPosture = PlayerCombatPosture.None;
			model.EquippedUpperBodyMode = UpperBodyMode.None;
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

		private static PlayerCombatPosture ResolveSecondaryPosture(PlayerModel model)
		{
			var effect = model.CombatLoadout.SecondarySlot.ModifierPostureEffect;

			return effect switch
			{
				PlayerModifierPostureEffect.Aim => PlayerCombatPosture.Aim,
				PlayerModifierPostureEffect.Block => PlayerCombatPosture.Block,
				_ => PlayerCombatPosture.None,
			};
		}

		private static UpperBodyMode ResolveUpperBodyMode(PlayerCombatPosture posture)
		{
			return posture switch
			{
				PlayerCombatPosture.Aim => UpperBodyMode.Aim,
				PlayerCombatPosture.Block => UpperBodyMode.Block,
				PlayerCombatPosture.SpellReady => UpperBodyMode.SpellReady,
				_ => UpperBodyMode.None,
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
				model.PrimaryMode = PrimaryModifierMode.None;
				model.SecondaryMode = SecondaryModifierMode.None;
				model.CombatPosture = PlayerCombatPosture.None;
				model.EquippedUpperBodyMode = UpperBodyMode.None;
			}
		}
	}
}
