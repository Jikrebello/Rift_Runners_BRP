using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class TraversalStaminaTests
	{
		[Test]
		public void BeginGlide_WhenStaminaIsZero_IsDenied()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				Stamina = 0f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(
				model,
				outputs,
				new List<IPlayerIntent> { new BeginGlideIntent() },
				dt: 0f
			);

			Assert.That(intents.Count, Is.EqualTo(0));
		}

		[Test]
		public void Drop_WhenStaminaIsSufficient_IsApproved_AndConsumesStamina()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel { Stamina = 20f, MaxStamina = 100f };
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(
				model,
				outputs,
				new List<IPlayerIntent> { new DropIntent() },
				dt: 0f
			);

			Assert.That(intents.Count, Is.EqualTo(1));
			Assert.That(intents[0], Is.TypeOf<DropIntent>());
			Assert.That(model.Stamina, Is.EqualTo(12f));
		}

		[Test]
		public void Drop_WhenStaminaTooLow_IsDenied()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel { Stamina = 4f, MaxStamina = 100f };
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(
				model,
				outputs,
				new List<IPlayerIntent> { new DropIntent() },
				dt: 0f
			);

			Assert.That(intents.Count, Is.EqualTo(0));
			Assert.That(model.Stamina, Is.EqualTo(4f));
		}

		[Test]
		public void Glide_DrainsStaminaOverTime()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				IsGliding = true,
				Stamina = 20f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			stamina.FilterAndApply(model, outputs, new List<IPlayerIntent>(), dt: 2f);

			Assert.That(model.Stamina, Is.EqualTo(6f));
		}

		[Test]
		public void Glide_WhenStaminaReachesZero_IsCancelled()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				IsGliding = true,
				Stamina = 4f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			stamina.FilterAndApply(model, outputs, new List<IPlayerIntent>(), dt: 1f);

			Assert.That(model.Stamina, Is.EqualTo(0f));
			Assert.That(model.IsGliding, Is.False);
		}

		[Test]
		public void KickOff_WhenStaminaIsSufficient_IsApproved_AndConsumesStamina()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel { Stamina = 20f, MaxStamina = 100f };
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(
				model,
				outputs,
				new List<IPlayerIntent> { new KickOffIntent() },
				dt: 0f
			);

			Assert.That(intents.Count, Is.EqualTo(1));
			Assert.That(intents[0], Is.TypeOf<KickOffIntent>());
			Assert.That(model.Stamina, Is.EqualTo(10f));
		}

		[Test]
		public void KickOff_WhenStaminaTooLow_IsDenied()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel { Stamina = 5f, MaxStamina = 100f };
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(
				model,
				outputs,
				new List<IPlayerIntent> { new KickOffIntent() },
				dt: 0f
			);

			Assert.That(intents.Count, Is.EqualTo(0));
			Assert.That(model.Stamina, Is.EqualTo(5f));
		}

		[Test]
		public void Leap_WhenStaminaIsSufficient_IsApproved_AndConsumesStamina()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel { Stamina = 30f, MaxStamina = 100f };
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(
				model,
				outputs,
				new List<IPlayerIntent> { new LeapIntent() },
				dt: 0f
			);

			Assert.That(intents.Count, Is.EqualTo(1));
			Assert.That(intents[0], Is.TypeOf<LeapIntent>());
			Assert.That(model.Stamina, Is.EqualTo(10f));
		}

		[Test]
		public void Leap_WhenStaminaTooLow_IsDenied()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel { Stamina = 10f, MaxStamina = 100f };
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(
				model,
				outputs,
				new List<IPlayerIntent> { new LeapIntent() },
				dt: 0f
			);

			Assert.That(intents.Count, Is.EqualTo(0));
			Assert.That(model.Stamina, Is.EqualTo(10f));
		}

		[Test]
		public void Sprint_DrainsStaminaOverTime()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sprinting,
				Stamina = 20f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(model, outputs, new List<IPlayerIntent>(), dt: 2f);

			Assert.That(intents.Count, Is.EqualTo(0));
			Assert.That(model.Stamina, Is.EqualTo(10f));
		}

		[Test]
		public void Sprint_WhenBelowMinimumThreshold_IsDenied()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Standing,
				Stamina = 5f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(
				model,
				outputs,
				new List<IPlayerIntent> { new ToggleSprintIntent() },
				dt: 0f
			);

			Assert.That(intents.Count, Is.EqualTo(0));
			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Standing));
			Assert.That(model.Stamina, Is.EqualTo(5f));
		}

		[Test]
		public void Sprint_WhenStaminaReachesZero_IsCancelled()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sprinting,
				Stamina = 4f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			stamina.FilterAndApply(model, outputs, new List<IPlayerIntent>(), dt: 1f);

			Assert.That(model.Stamina, Is.EqualTo(0f));
			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Standing));
		}

		private static StaminaConfig NewStaminaConfig()
		{
			return new StaminaConfig(
				maxStamina: 100f,
				regen: new RegenRates(groundedPerSecond: 8f, slidingPerSecond: 0f),
				drain: new DrainRates(sprintPerSecond: 5f, glidePerSecond: 7f),
				traversal: new TraversalCosts(
					leap: 20f,
					kickOff: 10f,
					drop: 8f,
					minStaminaToEnterSprint: 12f
				),
				skills: new SkillCosts(
					primarySkillSlot1Cost: 20f,
					primarySkillSlot2Cost: 25f,
					primarySkillSlot3Cost: 30f,
					secondarySkillSlot1Cost: 20f,
					secondarySkillSlot2Cost: 25f,
					secondarySkillSlot3Cost: 30f
				),
				exhaustion: new ExhaustionSettings(
					drainedMoveInitialMultiplier: 0.35f,
					drainedMoveDecayPerSecond: 4f,
					recoveryDurationSeconds: 1.0f,
					stopMoveThreshold: 0.05f
				)
			);
		}
	}
}
