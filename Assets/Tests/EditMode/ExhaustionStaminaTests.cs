using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Resolution;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class ExhaustionStaminaTests
	{
		[Test]
		public void RunningOutOfStaminaWhileMoving_EntersDrainedMoving()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sprinting,
				MoveInput = new Vector2(1f, 0f),
				Stamina = 4f,
				MaxStamina = 100f,
			};
			var outputs = new PlayerOutputs();

			stamina.FilterAndApply(model, outputs, new List<IPlayerIntent>(), dt: 0.8f);

			Assert.That(model.Stamina, Is.EqualTo(0f));
			Assert.That(model.ExhaustionState, Is.EqualTo(PlayerExhaustionState.DrainedMoving));
			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Standing));
			Assert.That(model.IsExhausted, Is.True);
			Assert.That(model.MoveInput.Length(), Is.GreaterThan(0f));
		}

		[Test]
		public void DrainedMoving_DecaysIntoRecovering()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				MoveInput = new Vector2(1f, 0f),
				Stamina = 0f,
				MaxStamina = 100f,
				ExhaustionState = PlayerExhaustionState.DrainedMoving,
			};
			var outputs = new PlayerOutputs();

			stamina.FilterAndApply(model, outputs, new List<IPlayerIntent>(), dt: 1f);

			Assert.That(model.ExhaustionState, Is.EqualTo(PlayerExhaustionState.Recovering));
			Assert.That(model.MoveInput, Is.EqualTo(Vector2.Zero));
			Assert.That(model.ExhaustionRecoveryRemaining, Is.GreaterThan(0f));
		}

		[Test]
		public void Recovering_DeniesMovementAndTraversalIntents()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = 0f,
				MaxStamina = 100f,
				ExhaustionState = PlayerExhaustionState.Recovering,
				ExhaustionRecoveryRemaining = 0.5f,
			};
			var outputs = new PlayerOutputs();

			var intents = stamina.FilterAndApply(
				model,
				outputs,
				new List<IPlayerIntent>
				{
					new MoveIntent(new Vector2(1f, 0f)),
					new ToggleSprintIntent(),
					new LeapIntent(),
				},
				dt: 0f
			);

			Assert.That(intents.Count, Is.EqualTo(0));
			Assert.That(model.MoveInput, Is.EqualTo(Vector2.Zero));
		}

		[Test]
		public void Recovering_DeniesResolvedCombatActions()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = 0f,
				MaxStamina = 100f,
				ExhaustionState = PlayerExhaustionState.Recovering,
				ExhaustionRecoveryRemaining = 0.5f,
			};
			var outputs = new PlayerOutputs();

			var request = new ResolvedPlayerActionRequest(PlayerActionDefinitions.LightAttack);
			var approved = stamina.FilterAndApplyResolvedAction(model, outputs, request);

			Assert.That(approved.HasValue, Is.False);
		}

		[Test]
		public void Recovering_CompletesAfterTimerAndStaminaReturnsAboveZero()
		{
			var stamina = new StaminaSystem(NewStaminaConfig());
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				Stamina = 0f,
				MaxStamina = 100f,
				ExhaustionState = PlayerExhaustionState.Recovering,
				ExhaustionRecoveryRemaining = 0.2f,
			};
			var outputs = new PlayerOutputs();

			stamina.FilterAndApply(model, outputs, new List<IPlayerIntent>(), dt: 1f);

			Assert.That(model.Stamina, Is.GreaterThan(0f));
			Assert.That(model.ExhaustionState, Is.EqualTo(PlayerExhaustionState.None));
			Assert.That(model.IsExhausted, Is.False);
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
					recoveryDurationSeconds: 0.5f,
					stopMoveThreshold: 0.05f
				)
			);
		}
	}
}
