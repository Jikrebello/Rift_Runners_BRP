using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class TraversalRulesTests
	{
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

			Assert.That(model.WantsSlideThisFrame, Is.True);

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f },
				new List<IPlayerIntent>()
			);

			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Sliding));
			Assert.That(
				outputs.Animation.Bools.Any(x => x.Param == AnimBool.Sliding && x.Value),
				Is.True
			);
			Assert.That(
				outputs.Animation.Bools.Any(x => x.Param == AnimBool.Sprinting && x.Value == false),
				Is.True
			);
		}

		[Test]
		public void Standing_TertiaryPressed_DoesNotStartSlide()
		{
			var fx = NewFixture();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Standing,
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

			Assert.That(model.WantsSlideThisFrame, Is.False);

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f },
				new List<IPlayerIntent>()
			);

			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Standing));
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

			Assert.That(outputs.Motor.RequestJump, Is.True);

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = false, PlanarSpeed = 1f },
				new List<IPlayerIntent>()
			);

			Assert.That(model.TraversalMode, Is.EqualTo(PlayerTraversalMode.Airborne));
		}

		[Test]
		public void Sliding_JumpPressed_DoesNotImmediatelyRequestGroundJump()
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

			fx.Sliding.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f },
				1f / 60f
			);

			Assert.That(outputs.Motor.RequestJump, Is.False);
			Assert.That(model.TraversalMode, Is.EqualTo(PlayerTraversalMode.Grounded));
			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Sliding));
		}

		[Test]
		public void Sliding_TapJump_SynthesizesKickOff_AndCoordinatorReturnsToSprinting()
		{
			var fx = NewFixture();
			var synthesizer = fx.Synthesizer;

			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sliding,
			};
			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Sliding, model, outputs);
			outputs.Clear();

			var pressed = synthesizer.Synthesize(
				model,
				new List<IPlayerIntent> { new JumpPressedIntent() },
				0.05f
			);

			Assert.That(pressed.Any(x => x is KickOffIntent), Is.False);
			Assert.That(pressed.Any(x => x is LeapIntent), Is.False);

			var released = synthesizer.Synthesize(
				model,
				new List<IPlayerIntent> { new JumpReleasedIntent() },
				0f
			);

			Assert.That(released.Any(x => x is KickOffIntent), Is.True);
			Assert.That(released.Any(x => x is LeapIntent), Is.False);

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 1f },
				released
			);

			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Sprinting));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.KickOffJump),
				Is.True
			);
		}

		[Test]
		public void Sliding_HoldJump_SynthesizesLeap_AndCoordinatorTransitionsToAirborne()
		{
			var fx = NewFixture();
			var synthesizer = fx.Synthesizer;

			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sliding,
			};
			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Sliding, model, outputs);
			outputs.Clear();

			var pressed = synthesizer.Synthesize(
				model,
				new List<IPlayerIntent> { new JumpPressedIntent() },
				0f
			);

			Assert.That(pressed.Any(x => x is LeapIntent), Is.False);
			Assert.That(pressed.Any(x => x is KickOffIntent), Is.False);

			var held = synthesizer.Synthesize(model, new List<IPlayerIntent>(), 0.30f);

			Assert.That(held.Any(x => x is LeapIntent), Is.True);
			Assert.That(held.Any(x => x is KickOffIntent), Is.False);

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = false, PlanarSpeed = 1f },
				held
			);

			Assert.That(model.TraversalMode, Is.EqualTo(PlayerTraversalMode.Airborne));
		}

		[Test]
		public void Sliding_ReleaseAfterLongHold_DoesNotAlsoSynthesizeKickOff()
		{
			var fx = NewFixture();
			var synthesizer = fx.Synthesizer;

			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sliding,
			};

			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Sliding, model, outputs);
			outputs.Clear();

			synthesizer.Synthesize(model, new List<IPlayerIntent> { new JumpPressedIntent() }, 0f);

			var held = synthesizer.Synthesize(model, new List<IPlayerIntent>(), 0.30f);
			Assert.That(held.Any(x => x is LeapIntent), Is.True);

			var released = synthesizer.Synthesize(
				model,
				new List<IPlayerIntent> { new JumpReleasedIntent() },
				0f
			);

			Assert.That(released.Any(x => x is KickOffIntent), Is.False);
			Assert.That(released.Any(x => x is LeapIntent), Is.False);
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

			fx.Sliding.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0.0f },
				1f / 60f
			);

			Assert.That(model.WantsExitSlideThisFrame, Is.True);

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0.0f },
				new List<IPlayerIntent>()
			);

			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Standing));
			Assert.That(model.WantsExitSlideThisFrame, Is.False);
		}

		[Test]
		public void Sliding_WhenSpeedIsAboveThreshold_DoesNotRequestExit()
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

			fx.Sliding.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 5.0f },
				1f / 60f
			);

			Assert.That(model.WantsExitSlideThisFrame, Is.False);
			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Sliding));
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
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				new List<IPlayerIntent>()
			);

			Assert.That(model.TraversalMode, Is.EqualTo(PlayerTraversalMode.Grounded));
			Assert.That(outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.Roll), Is.True);
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
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				new List<IPlayerIntent>()
			);

			Assert.That(outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.Roll), Is.False);
		}

		[Test]
		public void LandWhileStillAirborne_DoesNotTransition_WhenWorldIsNotGrounded()
		{
			var fx = NewFixture();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				AirborneEnterKind = AirborneEnterKind.Leap,
			};

			var outputs = new PlayerOutputs();

			fx.Traversal.TransitionTo(fx.Airborne, model, outputs);
			outputs.Clear();

			fx.Coordinator.ApplyTransitions(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = false, PlanarSpeed = 0f },
				new List<IPlayerIntent>()
			);

			Assert.That(model.TraversalMode, Is.EqualTo(PlayerTraversalMode.Airborne));
			Assert.That(outputs.Animation.Triggers.Count, Is.EqualTo(0));
		}

		private static TraversalFixture NewFixture() => new();
	}
}
