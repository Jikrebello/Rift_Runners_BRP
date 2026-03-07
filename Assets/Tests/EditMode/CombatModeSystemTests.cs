using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.CombatMode;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;
using Shouldly;

namespace Assets.Tests.EditMode
{
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
