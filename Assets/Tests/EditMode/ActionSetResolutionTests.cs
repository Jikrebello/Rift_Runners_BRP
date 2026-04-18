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
		public void ContextInteractIntent_ResolvesDirectlyToContextInteractAction()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.ContextInteractId = PlayerActionId.None;

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
		public void HeavyAttack_UsesConfiguredBaseBankHeavyAttack()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatLoadout.ActionSet.BaseBank.HeavyAttackId = PlayerActionId.HeavyAttack;

			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.HeavyAttack);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.HeavyAttack);
		}

		[Test]
		public void HeavyAttack_WhenMappedToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.HeavyAttackId = PlayerActionId.None;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.HasActiveAction.ShouldBeFalse();
			outputs.Animation.Triggers.Count.ShouldBe(0);
		}

		[Test]
		public void HeavyAttack_WithDualModifiers_UsesDualModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.DualModifierBank.HeavyAttackId = PlayerActionId.Skill2;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill2));
		}

		[Test]
		public void HeavyAttack_WithPrimaryModifier_UsesPrimaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.PrimaryModifierBank.HeavyAttackId = PlayerActionId.Skill2;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill2));
		}

		[Test]
		public void HeavyAttack_WithSecondaryModifier_UsesSecondaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.HeavyAttackId =
				PlayerActionId.Skill2;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill2));
		}

		[Test]
		public void LightAttack_UsesConfiguredBaseBankLightAttack()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatLoadout.ActionSet.BaseBank.LightAttackId = PlayerActionId.LightAttack;

			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.LightAttack);
		}

		[Test]
		public void LightAttack_WhenMappedToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatLoadout.ActionSet.BaseBank.LightAttackId = PlayerActionId.None;

			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.HasActiveAction.ShouldBeFalse();
			outputs.Animation.Triggers.Count.ShouldBe(0);
		}

		[Test]
		public void LightAttack_WithDualModifiers_UsesDualModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.DualModifierBank.LightAttackId =
				PlayerActionId.HeavyAttack;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);
		}

		[Test]
		public void LightAttack_WithoutModifiers_UsesBaseBank()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.LightAttackId = PlayerActionId.LightAttack;
			model.CombatLoadout.ActionSet.PrimaryModifierBank.LightAttackId = PlayerActionId.Skill1;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
		}

		[Test]
		public void LightAttack_WithPrimaryModifier_UsesPrimaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.LightAttackId = PlayerActionId.LightAttack;
			model.CombatLoadout.ActionSet.PrimaryModifierBank.LightAttackId = PlayerActionId.Skill1;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill1));
		}

		[Test]
		public void LightAttack_WithSecondaryModifier_UsesSecondaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.LightAttackId =
				PlayerActionId.Skill1;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill1));
		}

		[Test]
		public void RightAction_InBaseBank_UsesBaseRightActionMapping()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatLoadout.ActionSet.BaseBank.RightActionId = PlayerActionId.ContextGrab;

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
		public void RightAction_WhenBaseBankMappedToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatLoadout.ActionSet.BaseBank.RightActionId = PlayerActionId.None;

			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			model.ActionRuntime.HasActiveAction.ShouldBeFalse();
			outputs.Animation.Triggers.Count.ShouldBe(0);
		}

		[Test]
		public void RightAction_WhenPrimaryModifierBankMappedToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.PrimaryModifierBank.RightActionId = PlayerActionId.None;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(outputs.Animation.Triggers.Count, Is.EqualTo(0));
		}

		[Test]
		public void RightAction_WhenSecondaryModifierBankMappedToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.RightActionId = PlayerActionId.None;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(outputs.Animation.Triggers.Count, Is.EqualTo(0));
		}

		[Test]
		public void RightAction_WithDualModifiers_UsesDualModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.DualModifierBank.RightActionId = PlayerActionId.Skill3;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill3));
		}

		[Test]
		public void RightAction_WithPrimaryModifier_UsesPrimaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.PrimaryModifierBank.RightActionId = PlayerActionId.Skill3;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill3));
		}

		[Test]
		public void RightAction_WithSecondaryModifier_UsesSecondaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.RightActionId =
				PlayerActionId.FundamentalBlockPrimary;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.FundamentalBlockPrimary)
			);
			outputs.Animation.Triggers.ShouldContain(
				x => x.Param == AnimTrigger.FundamentalBlockPrimary
			);
		}

		[Test]
		public void RightAction_WithSecondaryModifier_UsesDefaultShieldFundamentalMapping()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.FundamentalBlockPrimary);
			outputs.Animation.Triggers.ShouldContain(
				x => x.Param == AnimTrigger.FundamentalBlockPrimary
			);
		}

		[Test]
		public void SkillSlot1_UsesCurrentBaseBankMapping()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatLoadout.ActionSet.BaseBank.SkillSlot1Id = PlayerActionId.Skill2;

			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.Skill2);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.Skill2);
		}

		[Test]
		public void SkillSlot1_WhenMappedToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatLoadout.ActionSet.BaseBank.SkillSlot1Id = PlayerActionId.None;

			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			model.ActionRuntime.HasActiveAction.ShouldBeFalse();
			outputs.Animation.Triggers.Count.ShouldBe(0);
		}

		[Test]
		public void SkillSlot1_WithDualModifiers_UsesDualModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.DualModifierBank.SkillSlot1Id = PlayerActionId.Skill3;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill3));
		}

		[Test]
		public void SkillSlot1_WithPrimaryModifier_UsesPrimaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.PrimaryModifierBank.SkillSlot1Id = PlayerActionId.Skill1;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill1));
		}

		[Test]
		public void SkillSlot1_WithSecondaryModifier_UsesSecondaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.SkillSlot1Id =
				PlayerActionId.Skill2;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Secondary, 1) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill2));
		}

		[Test]
		public void SkillSlot2_UsesCurrentBaseBankMapping()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.SkillSlot2Id = PlayerActionId.Skill3;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 2) },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.Skill3);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.Skill3);
		}

		[Test]
		public void SkillSlot2_WithDualModifiers_UsesDualModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.DualModifierBank.SkillSlot2Id = PlayerActionId.Skill2;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 2) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill2));
		}

		[Test]
		public void SkillSlot2_WithPrimaryModifier_UsesPrimaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.PrimaryModifierBank.SkillSlot2Id = PlayerActionId.Skill2;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 2) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill2));
		}

		[Test]
		public void SkillSlot2_WithSecondaryModifier_UsesSecondaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.SkillSlot2Id =
				PlayerActionId.Skill2;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Secondary, 2) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill2));
		}

		[Test]
		public void SkillSlot3_UsesCurrentBaseBankMapping()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.SkillSlot3Id = PlayerActionId.Skill1;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 3) },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.Skill1);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.Skill1);
		}

		[Test]
		public void SkillSlot3_WithDualModifiers_UsesDualModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.DualModifierBank.SkillSlot3Id = PlayerActionId.Skill3;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 3) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill3));
		}

		[Test]
		public void SkillSlot3_WithPrimaryModifier_UsesPrimaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.PrimaryModifierBank.SkillSlot3Id = PlayerActionId.Skill3;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 3) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill3));
		}

		[Test]
		public void SkillSlot3_WithSecondaryModifier_UsesSecondaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.SkillSlot3Id =
				PlayerActionId.Skill3;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Secondary, 3) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill3));
		}

		private static ActionTestDriver NewSystem() => new();
	}
}
