using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Resolution;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class ActionStaminaTests
	{
		[Test]
		public void HeavyAttack_WhenStaminaAboveZero_IsApproved_AndDoesNotConsumeStamina()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = 25f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			Assert.That(
				resolver.TryResolve(
					model,
					new[] { new HeavyAttackIntent() as IPlayerIntent },
					out var request
				),
				Is.True
			);

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			Assert.That(approved.HasValue, Is.True);
			Assert.That(approved.Value.Action.Id, Is.EqualTo(PlayerActionId.HeavyAttack));
			Assert.That(model.Stamina, Is.EqualTo(25f));
		}

		[Test]
		public void HeavyAttack_WhenStaminaIsZero_IsDenied()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = 0f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			Assert.That(
				resolver.TryResolve(
					model,
					new[] { new HeavyAttackIntent() as IPlayerIntent },
					out var request
				),
				Is.True
			);

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			Assert.That(approved.HasValue, Is.False);
			Assert.That(model.Stamina, Is.EqualTo(0f));
		}

		[Test]
		public void LightAttack_WhenStaminaAboveZero_IsApproved_AndDoesNotConsumeStamina()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = 25f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			Assert.That(
				resolver.TryResolve(
					model,
					new[] { new LightAttackIntent() as IPlayerIntent },
					out var request
				),
				Is.True
			);

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			Assert.That(approved.HasValue, Is.True);
			Assert.That(approved.Value.Action.Id, Is.EqualTo(PlayerActionId.LightAttack));
			Assert.That(model.Stamina, Is.EqualTo(25f));
		}

		[Test]
		public void LightAttack_WhenStaminaIsZero_IsDenied()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = 0f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			Assert.That(
				resolver.TryResolve(
					model,
					new[] { new LightAttackIntent() as IPlayerIntent },
					out var request
				),
				Is.True
			);

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			Assert.That(approved.HasValue, Is.False);
			Assert.That(model.Stamina, Is.EqualTo(0f));
		}

		[Test]
		public void SkillAction_WhenStaminaIsSufficient_IsApproved_AndConsumesStamina()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = 25f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.SkillSlot1Id = PlayerActionId.Skill1;

			Assert.That(
				resolver.TryResolve(
					model,
					new[] { new UseSkillIntent(SkillBank.Primary, 1) as IPlayerIntent },
					out var request
				),
				Is.True
			);

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			Assert.That(approved.HasValue, Is.True);
			Assert.That(approved.Value.Action.Id, Is.EqualTo(PlayerActionId.Skill1));
			Assert.That(model.Stamina, Is.EqualTo(5f));
		}

		[Test]
		public void SkillAction_WhenStaminaTooLow_IsDenied()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = 5f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.SkillSlot1Id = PlayerActionId.Skill1;

			Assert.That(
				resolver.TryResolve(
					model,
					new[] { new UseSkillIntent(SkillBank.Primary, 1) as IPlayerIntent },
					out var request
				),
				Is.True
			);

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			Assert.That(approved.HasValue, Is.False);
			Assert.That(model.Stamina, Is.EqualTo(5f));
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
