using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;
using Shouldly;

namespace Assets.Tests.EditMode
{
	/// <summary>
	/// Contains unit tests that verify the behavior of the action system, ensuring correct handling of player actions,
	/// action buffering, and state transitions within the game environment.
	/// </summary>
	/// <remarks>These tests validate that the action system processes player intents as expected, manages action
	/// buffering appropriately, and updates the action runtime state accurately after each step. The suite covers
	/// scenarios such as action chaining, buffering rules, and completion of actions to help maintain reliable gameplay
	/// mechanics.</remarks>
	public sealed class ActionSystemTests
	{
		[Test]
		public void BufferedAction_DoesNotGetOverwritten_WhenBufferAlreadyOccupied()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.HeavyAttack);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.HeavyAttack);
		}

		[Test]
		public void BufferedAction_StartsAfterCurrentActionCompletes()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.HeavyAttack);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.23f);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.HeavyAttack);
			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Startup);
			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.None);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.HeavyAttack);
		}

		[Test]
		public void FundamentalAction_WhileBusy_CanBuffer_WhenCurrentActionAllowsBuffering()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.HeavyAttack);
		}

		[Test]
		public void LightAttack_AdvancesThroughStartupActiveRecovery()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Startup);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);

			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Active);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);

			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Recovery);
		}

		[Test]
		public void LightAttack_CompletesAndClearsRuntime()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.23f);

			model.ActionRuntime.HasActiveAction.ShouldBeFalse();
			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.None);
			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.None);
		}

		[Test]
		public void LightAttack_WhenIdle_StartsImmediately()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack);
			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Startup);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.LightAttack);
		}

		[Test]
		public void NoRequestedAction_AndNoBufferedAction_LeavesRuntimeIdle()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.1f);

			model.ActionRuntime.HasActiveAction.ShouldBeFalse();
			model.ActionRuntime.HasBufferedAction.ShouldBeFalse();
		}

		[Test]
		public void RepeatedSameActionRequest_WhileBusy_FillsEmptyBuffer_WithoutOverwritingIt()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			// Start first action immediately.
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack);
			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.None);

			// Repeating the same request while busy should fill the empty buffer.
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack);
			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.LightAttack);

			// Once buffer is occupied, later requests should not overwrite it.
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack);
			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.LightAttack);
		}

		[Test]
		public void SecondAction_WhileBusy_IsBuffered()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack);
			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.HeavyAttack);
		}

		[Test]
		public void SkillAction_WhileBusy_DoesNotBuffer_WhenCurrentActionCannotBuffer()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.Skill1);
			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.None);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.Skill1);
			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.None);
		}

		[Test]
		public void RepeatedLightAttackInput_ChainsLightAttackIntoLightAttack2()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.LightAttack);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.23f);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack2);
			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Startup);
		}

		[Test]
		public void RepeatedLightAttackInput_ChainsLightAttack2IntoLightAttack3()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			// Start attack 1
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			// Buffer attack 1 request to promote into attack 2
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			// Complete attack 1
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.23f);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack2);

			// Buffer another light request while attack 2 is running
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(PlayerActionId.LightAttack);

			// Complete attack 2
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.10f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.21f);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.LightAttack3);
			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Startup);
		}

		[Test]
		public void FundamentalRangedPrimary_RepeatsIntoItself_WhenBufferedAgain()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.Aim,
			};
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.FundamentalRangedPrimary);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			model.ActionRuntime.BufferedRequestedActionId.ShouldBe(
				PlayerActionId.FundamentalRangedPrimary
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.07f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.15f);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.FundamentalRangedPrimary);
			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Startup);
		}

		private static ActionTestDriver NewSystem() => new();
	}
}
