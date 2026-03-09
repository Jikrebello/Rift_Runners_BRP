using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using NUnit.Framework;
using Shouldly;

namespace Assets.Tests.EditMode
{
	public sealed class TraversalRulesTests
	{
		private static TraversalFixture NewFixture() => new();

		[Test]
		public void Sprinting_TertiaryPressed_StartsSlide_ViaCoordinator()
		{
			var fx = NewFixture();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sprinting,
			};
			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Grounded, model, outputs);
			outputs.Clear();

			fx.Grounded.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new TertiaryPressedIntent() }
			);
			fx.Grounded.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f },
				1f / 60f
			);

			model.WantsSlideThisFrame.ShouldBeTrue();

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f }
			);

			model.GroundedSubMode.ShouldBe(PlayerGroundedSubMode.Sliding);

			outputs.Animation.Bools.ShouldContain(x => x.Param == AnimBool.Sliding && x.Value);
			outputs.Animation.Bools.ShouldContain(x => x.Param == AnimBool.Sprinting && !x.Value);
		}

		[Test]
		public void Sliding_TapJump_KickOffBackToSprinting_EmitsKickOffTrigger()
		{
			var fx = NewFixture();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sliding,
			};
			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Sliding, model, outputs);
			outputs.Clear();

			// Tap: press then release quickly (within KickOffTapMaxSeconds)
			fx.Sliding.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new JumpPressedIntent() }
			);

			fx.Sliding.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f },
				0.05f
			);

			fx.Sliding.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new JumpReleasedIntent() }
			);

			model.WantsKickOffThisFrame.ShouldBeTrue();
			model.WantsLeapThisFrame.ShouldBeFalse();

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f }
			);

			model.GroundedSubMode.ShouldBe(PlayerGroundedSubMode.Sprinting);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.KickOffJump);
		}

		[Test]
		public void Sliding_HoldJump_LeapsToAirborne_NoKickOff()
		{
			var fx = NewFixture();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sliding,
			};
			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Sliding, model, outputs);
			outputs.Clear();

			fx.Sliding.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new JumpPressedIntent() }
			);

			// Hold long enough to pass LeapHoldMinSeconds
			fx.Sliding.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f },
				0.30f
			);

			model.WantsLeapThisFrame.ShouldBeTrue();
			model.WantsKickOffThisFrame.ShouldBeFalse();

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = false, PlanarSpeed = 1f }
			);

			model.TraversalMode.ShouldBe(PlayerTraversalMode.Airborne);
		}

		[Test]
		public void Sprinting_JumpPressed_TransitionsToAirborne_ViaCoordinator()
		{
			var fx = NewFixture();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sprinting,
			};
			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Grounded, model, outputs);
			outputs.Clear();

			fx.Grounded.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new JumpPressedIntent() }
			);

			fx.Grounded.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f },
				1f / 60f
			);

			outputs.Motor.RequestJump.ShouldBeTrue();

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = false, PlanarSpeed = 1f }
			);

			model.TraversalMode.ShouldBe(PlayerTraversalMode.Airborne);
		}

		[Test]
		public void Sliding_WhenSpeedDropsBelowThreshold_RequestsExit_AndCoordinatorReturnsToStanding()
		{
			var fx = NewFixture();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sliding,
			};
			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Sliding, model, outputs);
			outputs.Clear();

			// SlidingState should flag exit when PlanarSpeed <= SlideStopSpeed
			fx.Sliding.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0.0f },
				1f / 60f
			);

			model.WantsExitSlideThisFrame.ShouldBeTrue();

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0.0f }
			);

			model.GroundedSubMode.ShouldBe(PlayerGroundedSubMode.Standing);
			model.WantsExitSlideThisFrame.ShouldBeFalse();
		}

		[Test]
		public void LandAfterLeap_WithNoAirOptionConsumed_TriggersRoll()
		{
			var fx = NewFixture();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				AirborneEnterKind = AirborneEnterKind.Leap,
				AirOptionConsumedThisAirborne = false,
			};

			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Airborne, model, outputs);
			outputs.Clear();

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f }
			);

			model.TraversalMode.ShouldBe(PlayerTraversalMode.Grounded);
			outputs.Animation.Triggers.ShouldContain(x => x.Param == AnimTrigger.Roll);
		}

		[Test]
		public void LandAfterLeap_WithAirOptionConsumed_DoesNotTriggerRoll()
		{
			var fx = NewFixture();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				AirborneEnterKind = AirborneEnterKind.Leap,
				AirOptionConsumedThisAirborne = true,
			};

			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Airborne, model, outputs);
			outputs.Clear();

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f }
			);

			outputs.Animation.Triggers.ShouldNotContain(x => x.Param == AnimTrigger.Roll);
		}
	}
}
