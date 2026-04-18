using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Characters.Core.Player.Action.Loadout;
using Assets.Scripts.Game.Characters.Core.Player.CombatMode;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class CombatModeSystemTests
	{
		[Test]
		public void HoldingPrimaryModifier_ActivatesPrimaryMode_WithoutChangingPosture()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				PrimaryMode = PrimaryModifierMode.None,
				SecondaryMode = SecondaryModifierMode.None,
				CombatPosture = PlayerCombatPosture.None,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new PrimaryModifierHeldIntent(true) }
			);

			Assert.That(model.PrimaryMode, Is.EqualTo(PrimaryModifierMode.Active));
			Assert.That(model.SecondaryMode, Is.EqualTo(SecondaryModifierMode.None));
			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.None));
			Assert.That(model.CombatStance, Is.EqualTo(PlayerCombatStance.Holstered));
		}

		[Test]
		public void ReleasingPrimaryModifier_ClearsPrimaryMode()
		{
			var model = new PlayerModel
			{
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.None,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new PrimaryModifierHeldIntent(false) }
			);

			Assert.That(model.PrimaryMode, Is.EqualTo(PrimaryModifierMode.None));
		}

		[Test]
		public void HoldingSecondaryModifier_AutoUnholsters_ActivatesSecondary()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
			};

			model.CombatLoadout.SecondarySlot.ModifierPostureEffect =
				PlayerModifierPostureEffect.Aim;

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(true) }
			);

			Assert.That(model.CombatStance, Is.EqualTo(PlayerCombatStance.Unholstered));
			Assert.That(model.SecondaryMode, Is.EqualTo(SecondaryModifierMode.Active));
			Assert.That(model.IsSecondaryModifierActive, Is.True);

			Assert.That(
				outputs.Animation.Bools.Any(x => x.Param == AnimBool.Holstered && !x.Value),
				Is.True
			);
			Assert.That(
				outputs.Animation.Bools.Any(x =>
					x.Param == AnimBool.SecondaryModifierActive && x.Value
				),
				Is.True
			);
		}

		[Test]
		public void HoldingSecondaryModifier_WithAimSlotEffect_SetsCombatPostureToAim()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
			};

			model.CombatLoadout.SecondarySlot.ModifierPostureEffect =
				PlayerModifierPostureEffect.Aim;

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(true) }
			);

			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.Aim));
			Assert.That(model.EquippedUpperBodyMode, Is.EqualTo(UpperBodyMode.Aim));
			Assert.That(
				outputs.Animation.Ints.Any(x =>
					x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.Aim
				),
				Is.True
			);
		}

		[Test]
		public void HoldingSecondaryModifier_WithBlockSlotEffect_SetsCombatPostureToBlock()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
			};

			model.CombatLoadout.SecondarySlot.ModifierPostureEffect =
				PlayerModifierPostureEffect.Block;

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(true) }
			);

			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.Block));
			Assert.That(model.EquippedUpperBodyMode, Is.EqualTo(UpperBodyMode.Block));
			Assert.That(
				outputs.Animation.Ints.Any(x =>
					x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.Block
				),
				Is.True
			);
		}

		[Test]
		public void HoldingSecondaryModifier_WithDefaultLoadout_SetsCombatPostureToBlock()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(true) }
			);

			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.Block));
			Assert.That(model.EquippedUpperBodyMode, Is.EqualTo(UpperBodyMode.Block));
		}

		[Test]
		public void HoldingSecondaryModifier_WithNoPostureEffect_KeepsCombatPostureNone()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				SecondaryMode = SecondaryModifierMode.None,
			};

			model.CombatLoadout.SecondarySlot.ModifierPostureEffect =
				PlayerModifierPostureEffect.None;

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(true) }
			);

			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.None));
			Assert.That(model.EquippedUpperBodyMode, Is.EqualTo(UpperBodyMode.None));
			Assert.That(
				outputs.Animation.Ints.Any(x =>
					x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.None
				),
				Is.True
			);
		}

		[Test]
		public void ReleasingSecondaryModifier_LeavesUnholstered_DisablesSecondary_AndClearsPosture()
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

			Assert.That(model.CombatStance, Is.EqualTo(PlayerCombatStance.Unholstered));
			Assert.That(model.SecondaryMode, Is.EqualTo(SecondaryModifierMode.None));
			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.None));
			Assert.That(model.EquippedUpperBodyMode, Is.EqualTo(UpperBodyMode.None));

			Assert.That(
				outputs.Animation.Bools.Any(x =>
					x.Param == AnimBool.SecondaryModifierActive && !x.Value
				),
				Is.True
			);
			Assert.That(
				outputs.Animation.Ints.Any(x =>
					x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.None
				),
				Is.True
			);
		}

		[Test]
		public void HoldingPrimaryAndSecondaryModifiers_KeepsPrimaryActiveWhileApplyingSecondaryPosture()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				PrimaryMode = PrimaryModifierMode.None,
				SecondaryMode = SecondaryModifierMode.None,
			};

			model.CombatLoadout.SecondarySlot.ModifierPostureEffect =
				PlayerModifierPostureEffect.Aim;

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent>
				{
					new PrimaryModifierHeldIntent(true),
					new SecondaryModifierHeldIntent(true),
				}
			);

			Assert.That(model.PrimaryMode, Is.EqualTo(PrimaryModifierMode.Active));
			Assert.That(model.SecondaryMode, Is.EqualTo(SecondaryModifierMode.Active));
			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.Aim));
			Assert.That(model.EquippedUpperBodyMode, Is.EqualTo(UpperBodyMode.Aim));
			Assert.That(model.CombatStance, Is.EqualTo(PlayerCombatStance.Unholstered));
		}

		[Test]
		public void ReleasingSecondaryModifier_DoesNotClearPrimaryModifier()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Unholstered,
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.Active,
				CombatPosture = PlayerCombatPosture.Aim,
				EquippedUpperBodyMode = UpperBodyMode.Aim,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryModifierHeldIntent(false) }
			);

			Assert.That(model.PrimaryMode, Is.EqualTo(PrimaryModifierMode.Active));
			Assert.That(model.SecondaryMode, Is.EqualTo(SecondaryModifierMode.None));
			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.None));
			Assert.That(model.EquippedUpperBodyMode, Is.EqualTo(UpperBodyMode.None));
		}

		[Test]
		public void TogglingWeaponStance_ToHolstered_ClearsPrimarySecondaryAndPosture()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Unholstered,
				PrimaryMode = PrimaryModifierMode.Active,
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

			Assert.That(model.CombatStance, Is.EqualTo(PlayerCombatStance.Holstered));
			Assert.That(model.PrimaryMode, Is.EqualTo(PrimaryModifierMode.None));
			Assert.That(model.SecondaryMode, Is.EqualTo(SecondaryModifierMode.None));
			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.None));
			Assert.That(model.EquippedUpperBodyMode, Is.EqualTo(UpperBodyMode.None));

			Assert.That(
				outputs.Animation.Bools.Any(x => x.Param == AnimBool.Holstered && x.Value),
				Is.True
			);
			Assert.That(
				outputs.Animation.Bools.Any(x =>
					x.Param == AnimBool.SecondaryModifierActive && !x.Value
				),
				Is.True
			);
		}

		[Test]
		public void TogglingWeaponStance_ToUnholstered_DoesNotForceModifiersOrPosture()
		{
			var model = new PlayerModel
			{
				CombatStance = PlayerCombatStance.Holstered,
				PrimaryMode = PrimaryModifierMode.None,
				SecondaryMode = SecondaryModifierMode.None,
				CombatPosture = PlayerCombatPosture.None,
				EquippedUpperBodyMode = UpperBodyMode.None,
			};

			var outputs = new PlayerOutputs();
			var sys = new CombatModeSystem();

			sys.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new ToggleWeaponStanceIntent() }
			);

			Assert.That(model.CombatStance, Is.EqualTo(PlayerCombatStance.Unholstered));
			Assert.That(model.PrimaryMode, Is.EqualTo(PrimaryModifierMode.None));
			Assert.That(model.SecondaryMode, Is.EqualTo(SecondaryModifierMode.None));
			Assert.That(model.CombatPosture, Is.EqualTo(PlayerCombatPosture.None));
			Assert.That(model.EquippedUpperBodyMode, Is.EqualTo(UpperBodyMode.None));

			Assert.That(
				outputs.Animation.Bools.Any(x => x.Param == AnimBool.Holstered && !x.Value),
				Is.True
			);
			Assert.That(
				outputs.Animation.Bools.Any(x =>
					x.Param == AnimBool.SecondaryModifierActive && !x.Value
				),
				Is.True
			);
		}
	}
}
