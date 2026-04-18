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
	public sealed class ActionCancelTests
	{
		[Test]
		public void LightAttack_RecoveryHeavyAttackRequest_CancelsImmediatelyIntoHeavyAttack()
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
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(model.ActionRuntime.HasBufferedAction, Is.False);
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.HeavyAttack),
				Is.True
			);
		}

		[Test]
		public void LightAttack_ActiveHeavyAttackRequest_DoesNotCancel_AndUsesNormalBufferBehavior()
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
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);
		}

		[Test]
		public void LightAttack_RecoveryLightAttackRequest_DoesNotUseCancelPolicy_AndStillUsesExistingChainBehavior()
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
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.23f);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack2)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
		}

		[Test]
		public void HeavyAttack_RecoveryLightAttackRequest_DoesNotCancel_WhenNoCancelPolicyExists()
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

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Recovery));

			outputs.Clear();
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
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Recovery));
			Assert.That(
				model.ActionRuntime.BufferedRequestedActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
		}

		[Test]
		public void ShieldGuardBash_RecoveryRightActionRequest_CancelsImmediatelyIntoFundamentalBlockPrimary()
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
				new List<IPlayerIntent> { new PrimaryPressedIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Recovery));
			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.ShieldGuardBash)
			);

			outputs.Clear();
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
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(model.ActionRuntime.HasBufferedAction, Is.False);
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.FundamentalBlockPrimary),
				Is.True
			);
		}

		[Test]
		public void ShieldGuardBash_ActiveRightActionRequest_DoesNotCancelBeforeRecovery()
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
				new List<IPlayerIntent> { new PrimaryPressedIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.ShieldGuardBash)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Active));
			Assert.That(model.ActionRuntime.HasBufferedAction, Is.False);
		}

		private static ActionTestDriver NewSystem() => new();
	}
}
