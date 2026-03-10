using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Resolution;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina;
using NUnit.Framework;
using Shouldly;

namespace Assets.Tests.EditMode
{
	public sealed class ActionStaminaTests
	{
		[Test]
		public void HeavyAttack_WhenStaminaIsSufficient_IsApproved_AndConsumesStamina()
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

			resolver
				.TryResolve(
					model,
					new[] { new HeavyAttackIntent() as IPlayerIntent },
					out var request
				)
				.ShouldBeTrue();

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			approved.HasValue.ShouldBeTrue();
			approved.Value.Action.Id.ShouldBe(PlayerActionId.HeavyAttack);
			model.Stamina.ShouldBe(15f);
		}

		[Test]
		public void HeavyAttack_WhenStaminaTooLow_IsDenied()
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

			resolver
				.TryResolve(
					model,
					new[] { new HeavyAttackIntent() as IPlayerIntent },
					out var request
				)
				.ShouldBeTrue();

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			approved.HasValue.ShouldBeFalse();
			model.Stamina.ShouldBe(5f);
		}

		[Test]
		public void LightAttack_WithZeroCost_IsApproved_AndDoesNotConsumeStamina()
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

			resolver
				.TryResolve(
					model,
					new[] { new LightAttackIntent() as IPlayerIntent },
					out var request
				)
				.ShouldBeTrue();

			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			approved.HasValue.ShouldBeTrue();
			model.Stamina.ShouldBe(25f);
		}

		private static StaminaConfig NewStaminaConfig()
		{
			return new StaminaConfig(
				maxStamina: 100f,
				regen: new RegenRates(groundedPerSecond: 0f, slidingPerSecond: 0f),
				drain: new DrainRates(sprintPerSecond: 0f, glidePerSecond: 0f),
				traversal: new TraversalCosts(
					leap: 20f,
					kickOff: 10f,
					minStaminaToEnterSprint: 12f
				),
				skills: new SkillCosts(0f, 0f, 0f, 0f, 0f, 0f)
			);
		}
	}
}
