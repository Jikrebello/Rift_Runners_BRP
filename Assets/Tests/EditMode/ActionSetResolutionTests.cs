using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;
using Shouldly;

namespace Assets.Tests.EditMode
{
	public sealed class ActionSetResolutionTests
	{
		[Test]
		public void PrimaryPressed_WithoutModifiers_UsesBaseBankPrimaryFaceAction()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new PrimaryPressedIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.LightAttack);
		}

		[Test]
		public void SecondaryPressed_WithoutModifiers_UsesBaseBankSecondaryFaceAction()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryPressedIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.HeavyAttack);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.HeavyAttack);
		}

		[Test]
		public void CombatTertiaryPressed_WithoutModifiers_UsesBaseBankTertiaryFaceAction()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new CombatTertiaryPressedIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.ContextInteract);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.ContextInteract);
		}

		[Test]
		public void ContextInteractIntent_StillResolvesDirectlyToContextInteractAction()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new ContextInteractIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.ContextInteract);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.ContextInteract);
		}

		[Test]
		public void PrimaryModifier_PrimarySecondaryTertiary_MapToSwordFaceBank()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			model.PrimaryMode = PrimaryModifierMode.Active;
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new PrimaryPressedIntent() },
				dt: 0f
			);
			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.SwordAdvanceSlash);

			model.ActionRuntime.ClearCurrent();
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryPressedIntent() },
				dt: 0f
			);
			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.SwordSkillSecondary);

			model.ActionRuntime.ClearCurrent();
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new CombatTertiaryPressedIntent() },
				dt: 0f
			);
			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.SwordSkillTertiary);
		}

		[Test]
		public void SecondaryModifier_PrimarySecondaryTertiary_MapToShieldFaceBank()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			model.SecondaryMode = SecondaryModifierMode.Active;
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new PrimaryPressedIntent() },
				dt: 0f
			);
			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.ShieldGuardBash);

			model.ActionRuntime.ClearCurrent();
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new SecondaryPressedIntent() },
				dt: 0f
			);
			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.ShieldSkillSecondary);

			model.ActionRuntime.ClearCurrent();
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new CombatTertiaryPressedIntent() },
				dt: 0f
			);
			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.ShieldSkillTertiary);
		}

		[Test]
		public void BothModifiersHeld_SecondaryBankWinsForFaceButtons()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			model.PrimaryMode = PrimaryModifierMode.Active;
			model.SecondaryMode = SecondaryModifierMode.Active;
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new PrimaryPressedIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.ShieldGuardBash);
		}

		[Test]
		public void RightAction_WithoutModifiers_UsesBaseRightActionMapping()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.ContextGrab);
		}

		[Test]
		public void RightAction_WithPrimaryModifier_UsesConfiguredPrimaryModifierRightAction()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			model.PrimaryMode = PrimaryModifierMode.Active;
			model.CombatLoadout.ActionSet.PrimaryModifierBank.RightActionId =
				PlayerActionId.SwordSkillTertiary;
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.SwordSkillTertiary);
		}

		[Test]
		public void RightAction_WithSecondaryModifier_UsesDefaultShieldFundamentalMapping()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			model.SecondaryMode = SecondaryModifierMode.Active;
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.FundamentalBlockPrimary);
			outputs.Animation.Triggers.ShouldContain(x =>
				x.Param == AnimTrigger.FundamentalBlockPrimary
			);
		}

		[Test]
		public void CombatTertiaryPressed_WhenMappedToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = NewGroundedModel();
			model.CombatLoadout.ActionSet.BaseBank.TertiaryFaceActionId = PlayerActionId.None;
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new CombatTertiaryPressedIntent() },
				dt: 0f
			);

			model.ActionRuntime.HasActiveAction.ShouldBeFalse();
			outputs.Animation.Triggers.Count.ShouldBe(0);
		}

		private static PlayerModel NewGroundedModel()
		{
			return new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
		}

		private static ActionTestDriver NewSystem() => new();
	}
}
