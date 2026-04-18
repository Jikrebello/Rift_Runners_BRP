using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Assets.Scripts.Game.Characters.Core.Player;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Input;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina;
using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class GroundedBlockTraversalTests
	{
		private static readonly Vector2 FullMoveInput = new(1f, 0f);
		private static readonly Vector2 DefaultExpectedMove =
			FullMoveInput * GroundedTraversalConfig.DefaultMoveInputMultiplier;
		private static readonly Vector2 BlockExpectedMove =
			FullMoveInput * GroundedTraversalConfig.DefaultBlockedMoveInputMultiplier;
		private static readonly Vector2 ExpectedBlockDashDirection = Vector2.Normalize(
			FullMoveInput
		);
		private static readonly Vector2 ExpectedBlockDashMove =
			ExpectedBlockDashDirection * GroundedTraversalConfig.DefaultBlockDashMoveMultiplier;
		private static readonly Vector2 ExpectedActionMove =
			ExpectedBlockDashDirection
			* PlayerActionDefinitions.SwordAdvanceSlash.Motor.MoveMultiplier;

		[Test]
		public void GroundedState_BlockPosture_UsesBlockedMovementProfile()
		{
			var fx = NewFixture();
			var model = NewGroundedModel();
			model.CombatPosture = PlayerCombatPosture.Block;
			var outputs = new PlayerOutputs();

			fx.Grounded.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new MoveIntent(FullMoveInput) }
			);
			fx.Grounded.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				1f / 60f
			);

			Assert.That(outputs.Motor.DesiredMove, Is.EqualTo(BlockExpectedMove));
			Assert.That(
				outputs.Animation.Floats.Last(x => x.Param == AnimFloat.Speed).Value,
				Is.EqualTo(BlockExpectedMove.Length())
			);
		}

		[Test]
		public void GroundedState_NonBlockPosture_UsesDefaultMovementProfile()
		{
			var fx = NewFixture();
			var model = NewGroundedModel();
			var outputs = new PlayerOutputs();

			fx.Grounded.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new MoveIntent(FullMoveInput) }
			);
			fx.Grounded.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				1f / 60f
			);

			Assert.That(outputs.Motor.DesiredMove, Is.EqualTo(DefaultExpectedMove));
			Assert.That(
				outputs.Animation.Floats.Last(x => x.Param == AnimFloat.Speed).Value,
				Is.EqualTo(DefaultExpectedMove.Length())
			);
		}

		[Test]
		public void GroundedState_BlockPosture_ToggleSprintIntent_IsIgnored()
		{
			var fx = NewFixture();
			var model = NewGroundedModel();
			model.CombatPosture = PlayerCombatPosture.Block;
			var outputs = new PlayerOutputs();

			fx.Grounded.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new ToggleSprintIntent() }
			);
			fx.Grounded.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				1f / 60f
			);

			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Standing));
			Assert.That(
				outputs.Animation.Bools.Last(x => x.Param == AnimBool.Sprinting).Value,
				Is.False
			);
		}

		[Test]
		public void GroundedState_BlockPosture_WhileAlreadySprinting_ExitsSprint()
		{
			var fx = NewFixture();
			var model = NewGroundedModel();
			model.CombatPosture = PlayerCombatPosture.Block;
			model.GroundedSubMode = PlayerGroundedSubMode.Sprinting;
			var outputs = new PlayerOutputs();

			fx.Grounded.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				1f / 60f
			);

			Assert.That(model.GroundedSubMode, Is.EqualTo(PlayerGroundedSubMode.Standing));
			Assert.That(
				outputs.Animation.Bools.Last(x => x.Param == AnimBool.Sprinting).Value,
				Is.False
			);
		}

		[Test]
		public void PlayerPiece_DefaultSwordShield_SecondaryModifierHeld_UsesBlockedMovementProfile()
		{
			var piece = NewPlayerPiece();
			var outputs = piece.Tick(
				new PlayerInputSnapshot
				{
					Move = FullMoveInput,
					SecondarySkillModifier = new ButtonState
					{
						Held = true,
						PressedThisFrame = true,
					},
				},
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				1f / 60f
			);

			Assert.That(outputs.Motor.DesiredMove, Is.EqualTo(BlockExpectedMove));
			Assert.That(
				outputs.Animation.Ints.Any(x =>
					x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.Block
				),
				Is.True
			);
		}

		[Test]
		public void PlayerPiece_DefaultSwordShield_SecondaryModifierHeld_PreventsSprint()
		{
			var piece = NewPlayerPiece();
			var outputs = piece.Tick(
				new PlayerInputSnapshot
				{
					SecondarySkillModifier = new ButtonState
					{
						Held = true,
						PressedThisFrame = true,
					},
					ToggleSprint = new ButtonState { PressedThisFrame = true },
				},
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				1f / 60f
			);

			Assert.That(
				outputs.Animation.Bools.Any(x => x.Param == AnimBool.Sprinting && x.Value),
				Is.False
			);
			Assert.That(
				outputs.Animation.Ints.Any(x =>
					x.Param == AnimInt.UpperBodyMode && x.Value == (int)UpperBodyMode.Block
				),
				Is.True
			);
		}

		[Test]
		public void GroundedState_BlockPosture_JumpWithMoveInput_RequestsBlockDash_NotJump()
		{
			var fx = NewFixture();
			var model = NewGroundedModel();
			model.CombatPosture = PlayerCombatPosture.Block;
			model.SecondaryMode = SecondaryModifierMode.Active;
			var outputs = new PlayerOutputs();

			fx.Grounded.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new MoveIntent(FullMoveInput), new JumpPressedIntent() }
			);
			fx.Grounded.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				1f / 60f
			);

			Assert.That(outputs.Motor.RequestBlockDashThisFrame, Is.True);
			Assert.That(outputs.Motor.BlockDashDirection, Is.EqualTo(ExpectedBlockDashDirection));
			Assert.That(outputs.Motor.BlockDashMove, Is.EqualTo(ExpectedBlockDashMove));
			Assert.That(outputs.Motor.RequestJump, Is.False);
			Assert.That(
				outputs.Animation.Triggers.Last(x => x.Param == AnimTrigger.BlockDash).Param,
				Is.EqualTo(AnimTrigger.BlockDash)
			);
		}

		[Test]
		public void GroundedState_BlockPosture_JumpWithoutMoveInput_RemainsNormalJump()
		{
			var fx = NewFixture();
			var model = NewGroundedModel();
			model.CombatPosture = PlayerCombatPosture.Block;
			model.SecondaryMode = SecondaryModifierMode.Active;
			var outputs = new PlayerOutputs();

			fx.Grounded.HandleIntents(
				model,
				outputs,
				new List<IPlayerIntent> { new JumpPressedIntent() }
			);
			fx.Grounded.Tick(
				model,
				outputs,
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				1f / 60f
			);

			Assert.That(outputs.Motor.RequestBlockDashThisFrame, Is.False);
			Assert.That(outputs.Motor.RequestJump, Is.True);
		}

		[Test]
		public void PlayerPiece_DefaultSwordShield_SecondaryModifierHeld_JumpWithMoveInput_RequestsBlockDash()
		{
			var piece = NewPlayerPiece();
			var outputs = piece.Tick(
				new PlayerInputSnapshot
				{
					Move = FullMoveInput,
					Jump = new ButtonState { Held = true, PressedThisFrame = true },
					SecondarySkillModifier = new ButtonState
					{
						Held = true,
						PressedThisFrame = true,
					},
				},
				new PlayerWorldSnapshot { IsGrounded = true, PlanarSpeed = 0f },
				1f / 60f
			);

			Assert.That(outputs.Motor.RequestBlockDashThisFrame, Is.True);
			Assert.That(outputs.Motor.BlockDashDirection, Is.EqualTo(ExpectedBlockDashDirection));
			Assert.That(outputs.Motor.BlockDashMove, Is.EqualTo(ExpectedBlockDashMove));
			Assert.That(outputs.Motor.RequestJump, Is.False);
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.BlockDash),
				Is.True
			);
		}

		[Test]
		public void MotorCommands_ResolvePlanarMove_UsesBlockDashThenActionMoveThenDesiredMove()
		{
			var commands = new MotorCommands
			{
				DesiredMove = DefaultExpectedMove,
				ActionMove = ExpectedActionMove,
				RequestBlockDashThisFrame = true,
				BlockDashMove = ExpectedBlockDashMove,
			};

			Assert.That(commands.ResolvePlanarMove(), Is.EqualTo(ExpectedBlockDashMove));

			commands.RequestBlockDashThisFrame = false;
			Assert.That(commands.ResolvePlanarMove(), Is.EqualTo(ExpectedActionMove));

			commands.ActionMove = Vector2.Zero;
			Assert.That(commands.ResolvePlanarMove(), Is.EqualTo(DefaultExpectedMove));
		}

		private static TraversalFixture NewFixture()
		{
			return new TraversalFixture(GroundedTraversalConfig.Default);
		}

		private static PlayerModel NewGroundedModel()
		{
			return new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Standing,
			};
		}

		private static PlayerPiece NewPlayerPiece()
		{
			return new PlayerPiece(
				new SlidingStateConfig(
					kickOffTapMaxSeconds: 0.18f,
					leapHoldMinSeconds: 0.22f,
					slideStopSpeed: 0.35f
				),
				new StaminaConfig(
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
				)
			);
		}
	}
}
