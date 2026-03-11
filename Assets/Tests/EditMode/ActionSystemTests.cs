using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class ActionSystemTests
	{
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

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.LightAttack),
				Is.True
			);
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

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Recovery));
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

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.None));
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.None));
		}

		[Test]
		public void NoRequestedAction_AndNoBufferedAction_LeavesRuntimeIdle()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.1f);

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(model.ActionRuntime.HasBufferedAction, Is.False);
		}

		[Test]
		public void TickWithNoRequest_WhileActionIsActive_DoesNotClearCurrentActionEarly()
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
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.05f);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
		}

		[Test]
		public void SecondAction_WhileBusy_IsBuffered_AndValidBufferWindow()
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

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));

			outputs.Clear();
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
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);
		}

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
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);
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
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.23f);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.None)
			);
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.HeavyAttack),
				Is.True
			);
		}

		[Test]
		public void RepeatedSameActionRequest_WhileBusy_FillsEmptyBuffer_WithoutOverwritingIt()
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

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.None)
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));

			outputs.Clear();
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
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);

			outputs.Clear();
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
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
		}

		[Test]
		public void LightAttack_DoesNotBufferDuringStartup_WhenWindowStartsAtActive()
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

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasBufferedAction, Is.False);
		}

		[Test]
		public void LightAttack_CanBufferDuringActive()
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

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
		}

		[Test]
		public void LightAttack_CanAlsoBufferDuringRecovery()
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

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Recovery));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
		}

		[Test]
		public void HeavyAttack_DoesNotBufferDuringActive_WhenWindowIsRecoveryOnly()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.19f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasBufferedAction, Is.False);
		}

		[Test]
		public void HeavyAttack_CanBufferDuringRecovery_WhenWindowIsRecoveryOnly()
		{
			var system = NewSystem();
			var model = new PlayerModel { Stamina = 100f, MaxStamina = 100f };
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.19f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Recovery));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
		}

		[Test]
		public void SkillAction_WhileBusy_DoesNotBuffer_WhenCurrentActionCannotBuffer()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.SkillSlot1Id = PlayerActionId.Skill1;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new UseSkillIntent(SkillBank.Primary, 1) },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill1));
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.None)
			);

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill1));
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.None)
			);
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

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.23f);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack2)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
		}

		[Test]
		public void RepeatedLightAttackInput_ChainsLightAttack2IntoLightAttack3()
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
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.23f);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack2)
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.10f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.21f);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack3)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
		}

		[Test]
		public void LightAttack3_DoesNotBuffer_WhenExecutionPolicyDisallowsIt()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			model.ActionRuntime.CurrentActionId = PlayerActionId.LightAttack3;
			model.ActionRuntime.CurrentPhase = PlayerActionPhase.Recovery;
			model.ActionRuntime.PhaseElapsedSeconds = 0f;

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasBufferedAction, Is.False);
		}

		[Test]
		public void FundamentalRangedPrimary_RepeatsIntoItself_WhenBufferedAgain()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.RightActionId =
				PlayerActionId.FundamentalRangedPrimary;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.FundamentalRangedPrimary)
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.07f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Recovery));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.FundamentalRangedPrimary)
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.15f);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.FundamentalRangedPrimary)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
		}

		[Test]
		public void FundamentalRangedPrimary_DoesNotBufferDuringActive_WhenWindowIsRecoveryOnly()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.RightActionId =
				PlayerActionId.FundamentalRangedPrimary;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.07f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasBufferedAction, Is.False);
		}

		[Test]
		public void CompletingAction_WithNoBufferedFollowUp_LeavesRuntimeIdle()
		{
			var system = NewSystem();
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.19f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.36f);

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(model.ActionRuntime.HasBufferedAction, Is.False);
			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.None));
		}

		private static ActionTestDriver NewSystem() => new();
	}
}
