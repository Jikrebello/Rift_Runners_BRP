using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.CombatMode;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;
using Shouldly;

namespace Assets.Tests.EditMode
{
	/// <summary>
	/// Contains unit tests for the CombatModeSystem, verifying the behavior of combat mode transitions based on player
	/// intents.
	/// </summary>
	/// <remarks>These tests ensure that the CombatModeSystem correctly handles various player actions, such as
	/// holding or releasing secondary modifiers and toggling weapon stances, affecting the player's combat stance and
	/// upper body mode accordingly.</remarks>
	public sealed class CombatModeSystemTests
	{
		[Test]
		public void HoldingSecondaryModifier_AutoUnholsters_ActivatesSecondary_AndSetsUpperBodyMode()
		{
			// Arrange
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
				EquippedUpperBodyMode = UpperBodyMode.Aim,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			var intents = new List<IPlayerIntent> { new SecondaryModifierHeldIntent(true) };

			// Act
			sys.HandleIntents(model, outputs, intents);

			// Assert
			model.CombatStance.ShouldBe(PlayerCombatStance.Unholstered);
			model.IsSecondaryModifierActive.ShouldBeTrue();

			outputs.Animation.Bools.ShouldContain(x => x.Param == AnimBool.Holstered && !x.Value);
			outputs.Animation.Bools.ShouldContain(x =>
				x.Param == AnimBool.SecondaryModifierActive && x.Value
			);

			outputs.Animation.Ints.ShouldContain(x =>
				x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.Aim
			);
		}

		[Test]
		public void HoldingSecondaryModifier_WithAimUpperBody_SetsCombatPostureToAim()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
				EquippedUpperBodyMode = UpperBodyMode.Aim,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(true) }
			);

			model.CombatPosture.ShouldBe(PlayerCombatPosture.Aim);
		}

		[Test]
		public void HoldingSecondaryModifier_WithBlockUpperBody_SetsCombatPostureToBlock()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
				EquippedUpperBodyMode = UpperBodyMode.Block,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(true) }
			);

			model.CombatPosture.ShouldBe(PlayerCombatPosture.Block);
		}

		[Test]
		public void HoldingSecondaryModifier_WithSpellReadyUpperBody_SetsCombatPostureToSpellReady()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
				EquippedUpperBodyMode = UpperBodyMode.SpellReady,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(true) }
			);

			model.CombatPosture.ShouldBe(PlayerCombatPosture.SpellReady);
		}

		[Test]
		public void HolsteringWeapon_ClearsCombatPosture()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Unholstered,
				SecondaryMode = SecondaryModifierMode.Active,
				CombatPosture = PlayerCombatPosture.Aim,
				EquippedUpperBodyMode = UpperBodyMode.Aim,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new ToggleWeaponStanceIntent() }
			);

			model.CombatStance.ShouldBe(PlayerCombatStance.Holstered);
			model.CombatPosture.ShouldBe(PlayerCombatPosture.None);
		}

		[Test]
		public void ReleasingSecondaryModifier_ClearsCombatPosture()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Unholstered,
				SecondaryMode = SecondaryModifierMode.Active,
				CombatPosture = PlayerCombatPosture.Block,
				EquippedUpperBodyMode = UpperBodyMode.Block,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(false) }
			);

			model.CombatPosture.ShouldBe(PlayerCombatPosture.None);
		}

		[Test]
		public void ReleasingSecondaryModifier_LeavesUnholstered_DisablesSecondary_AndResetsUpperBodyMode()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Unholstered,
				SecondaryMode = SecondaryModifierMode.Active,
				EquippedUpperBodyMode = UpperBodyMode.Block,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			var intents = new List<IPlayerIntent> { new SecondaryModifierHeldIntent(false) };

			sys.HandleIntents(model, outputs, intents);

			model.CombatStance.ShouldBe(PlayerCombatStance.Unholstered);
			model.IsSecondaryModifierActive.ShouldBeFalse();

			outputs.Animation.Bools.ShouldContain(x =>
				x.Param == AnimBool.SecondaryModifierActive && !x.Value
			);

			outputs.Animation.Ints.ShouldContain(x =>
				x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.None
			);
		}

		[Test]
		public void TogglingWeaponStance_ToHolstered_ClearsSecondary_AndResetsUpperBodyMode()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Unholstered,
				SecondaryMode = SecondaryModifierMode.Active,
				EquippedUpperBodyMode = UpperBodyMode.Aim,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			var intents = new List<IPlayerIntent> { new ToggleWeaponStanceIntent() };

			sys.HandleIntents(model, outputs, intents);

			model.CombatStance.ShouldBe(PlayerCombatStance.Holstered);
			model.IsSecondaryModifierActive.ShouldBeFalse();

			outputs.Animation.Bools.ShouldContain(x => x.Param == AnimBool.Holstered && x.Value);
			outputs.Animation.Bools.ShouldContain(x =>
				x.Param == AnimBool.SecondaryModifierActive && !x.Value
			);

			outputs.Animation.Ints.ShouldContain(x =>
				x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.None
			);
		}

		[Test]
		public void TogglingWeaponStance_ToUnholstered_DoesNotForceSecondaryActive()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
				EquippedUpperBodyMode = UpperBodyMode.Block,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			var intents = new List<IPlayerIntent> { new ToggleWeaponStanceIntent() };

			sys.HandleIntents(model, outputs, intents);

			model.CombatStance.ShouldBe(PlayerCombatStance.Unholstered);
			model.IsSecondaryModifierActive.ShouldBeFalse();

			outputs.Animation.Bools.ShouldContain(x => x.Param == AnimBool.Holstered && !x.Value);
			outputs.Animation.Bools.ShouldContain(x =>
				x.Param == AnimBool.SecondaryModifierActive && !x.Value
			);

			outputs.Animation.Ints.ShouldContain(x =>
				x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.None
			);
		}
	}
}
