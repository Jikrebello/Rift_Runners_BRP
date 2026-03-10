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
	/// Contains unit tests for the action system context, verifying the correct handling of player actions under various
	/// conditions.
	/// </summary>
	/// <remarks>These tests validate that player actions are properly initiated, rejected, or resolved based on the
	/// player's traversal mode, combat posture, and modifiers. The tests ensure that the action system responds as
	/// expected to different player states and intents, supporting reliable gameplay behavior.</remarks>
	public sealed class ActionSystemContextTests
	{
		[Test]
		public void LightAttack_WhenAirborne_IsRejected()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Airborne };
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			model.ActionRuntime.HasActiveAction.ShouldBeFalse();
			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.None);
			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.None);
			outputs.Animation.Triggers.Count.ShouldBe(0);
		}

		[Test]
		public void LightAttack_WhenGrounded_StartsNormally()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
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
		public void RightAction_WhenAimPostureActive_ResolvesToFundamentalRangedPrimary()
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
			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Startup);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.ContextGrabOrFire);
		}

		[Test]
		public void RightAction_WhenBlockPostureActive_FallsBackToNeutralRightAction()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.Block,
			};
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
		public void RightAction_WhenSpellReadyPostureActive_FallsBackToNeutralRightAction()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.SpellReady,
			};
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
		public void RightAction_WithoutSecondaryModifier_ResolvesToContextGrab()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.None,
			};
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			model.ActionRuntime.CurrentActionId.ShouldBe(PlayerActionId.ContextGrab);
			model.ActionRuntime.CurrentPhase.ShouldBe(PlayerActionPhase.Startup);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.ContextGrabOrFire);
		}

		private static ActionTestDriver NewSystem() => new();
	}
}
