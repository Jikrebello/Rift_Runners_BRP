using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;
using Shouldly;

namespace Assets.Tests.EditMode
{
	/// <summary>
	/// Contains unit tests for verifying the behavior of action set resolution in a player model.
	/// </summary>
	/// <remarks>These tests ensure that the correct actions are triggered based on the player's current action set
	/// and traversal mode. Each test simulates different player intents and checks that the expected actions are executed,
	/// validating the action mapping logic.</remarks>
	public sealed class ActionSetResolutionTests
	{
		[Test]
		public void LightAttack_UsesConfiguredFundamentalLightAttack()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatProfile.ActionSet.NeutralBank.LightAttackId = PlayerActionId.HeavyAttack;

			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.HeavyAttack);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.HeavyAttack);
		}

		[Test]
		public void LightAttack_WhenMappedToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatProfile.ActionSet.NeutralBank.LightAttackId = PlayerActionId.None;

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
		public void NeutralRightAction_WhenMappedToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.None,
			};

			model.CombatProfile.ActionSet.NeutralBank.RightActionId = PlayerActionId.None;

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
		public void RightAction_InAimPosture_UsesSecondaryRightActionMapping()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.Aim,
			};

			model.CombatProfile.ActionSet.AimBank.RightActionId =
				PlayerActionId.FundamentalRangedPrimary;

			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.FundamentalRangedPrimary);
		}

		[Test]
		public void RightAction_InNeutral_UsesNeutralRightActionMapping()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.None,
			};

			model.CombatProfile.ActionSet.AimBank.RightActionId = PlayerActionId.ContextGrab;

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
		public void SkillSlot1_UsesCurrentActionSetMapping()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };

			model.CombatProfile.ActionSet.NeutralBank.SkillSlot1Id = PlayerActionId.Skill2;

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

			model.CombatProfile.ActionSet.NeutralBank.SkillSlot1Id = PlayerActionId.None;

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
		public void LightAttack_InAimPosture_UsesAimBankLightAttack()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.Aim,
			};
			var outputs = new PlayerOutputs();

			model.CombatProfile.ActionSet.AimBank.LightAttackId = PlayerActionId.HeavyAttack;

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
		public void HeavyAttack_InBlockPosture_UsesBlockBankHeavyAttack()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.Block,
			};
			var outputs = new PlayerOutputs();

			model.CombatProfile.ActionSet.BlockBank.HeavyAttackId = PlayerActionId.LightAttack;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
		}

		[Test]
		public void SkillSlot1_InAimPosture_UsesAimBankSkillSlot1()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.Aim,
			};
			var outputs = new PlayerOutputs();

			model.CombatProfile.ActionSet.AimBank.SkillSlot1Id = PlayerActionId.Skill2;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill2));
		}

		[Test]
		public void SkillSlot1_InSpellReadyPosture_UsesSpellReadyBankSkillSlot1()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.SpellReady,
			};
			var outputs = new PlayerOutputs();

			model.CombatProfile.ActionSet.SpellReadyBank.SkillSlot1Id = PlayerActionId.Skill3;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill3));
		}

		private static ActionTestDriver NewSystem() => new();
	}
}
