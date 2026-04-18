using System.Linq;
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
			var model = NewModel(25f);
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
			var resolved = approved.GetValueOrDefault();

			Assert.That(approved.HasValue, Is.True);
			Assert.That(resolved.Action.Id, Is.EqualTo(PlayerActionId.HeavyAttack));
			Assert.That(model.Stamina, Is.EqualTo(25f));
		}

		[Test]
		public void HeavyAttack_WhenStaminaIsZero_IsDenied()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = NewModel(0f);
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
		public void SwordBankedSkill_WhenStaminaIsSufficient_IsApproved_AndConsumesStaminaOnce()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = NewModel(25f);
			model.PrimaryMode = PrimaryModifierMode.Active;
			var outputs = new PlayerOutputs();

			var filteredIntents = stamina.FilterAndApply(
				model,
				outputs,
				new[] { new PrimaryPressedIntent() as IPlayerIntent },
				dt: 0f
			);

			Assert.That(filteredIntents.Count, Is.EqualTo(1));
			Assert.That(filteredIntents.Single() is PrimaryPressedIntent, Is.True);
			Assert.That(resolver.TryResolve(model, filteredIntents, out var request), Is.True);

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);
			var resolved = approved.GetValueOrDefault();

			Assert.That(approved.HasValue, Is.True);
			Assert.That(resolved.Action.Id, Is.EqualTo(PlayerActionId.SwordAdvanceSlash));
			Assert.That(model.Stamina, Is.EqualTo(0f));
		}

		[Test]
		public void SwordBankedSkill_WhenStaminaTooLow_IsDenied_WithoutPartialSpend()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = NewModel(5f);
			model.PrimaryMode = PrimaryModifierMode.Active;
			var outputs = new PlayerOutputs();

			var filteredIntents = stamina.FilterAndApply(
				model,
				outputs,
				new[] { new PrimaryPressedIntent() as IPlayerIntent },
				dt: 0f
			);

			Assert.That(filteredIntents.Count, Is.EqualTo(1));
			Assert.That(resolver.TryResolve(model, filteredIntents, out var request), Is.True);

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			Assert.That(approved.HasValue, Is.False);
			Assert.That(model.Stamina, Is.EqualTo(5f));
		}

		[Test]
		public void ShieldGuardBash_WhenStaminaIsSufficient_IsApproved_AndConsumesStaminaOnce()
		{
			var resolver = new PlayerActionResolver();
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = NewModel(15f);
			model.SecondaryMode = SecondaryModifierMode.Active;
			var outputs = new PlayerOutputs();

			var filteredIntents = stamina.FilterAndApply(
				model,
				outputs,
				new[] { new PrimaryPressedIntent() as IPlayerIntent },
				dt: 0f
			);

			Assert.That(filteredIntents.Count, Is.EqualTo(1));
			Assert.That(resolver.TryResolve(model, filteredIntents, out var request), Is.True);

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);
			var resolved = approved.GetValueOrDefault();

			Assert.That(approved.HasValue, Is.True);
			Assert.That(resolved.Action.Id, Is.EqualTo(PlayerActionId.ShieldGuardBash));
			Assert.That(model.Stamina, Is.EqualTo(0f));
		}

		private static PlayerModel NewModel(float stamina)
		{
			return new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = stamina,
				MaxStamina = 100f,
			};
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
