using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using NUnit.Framework;
using Shouldly;

namespace Assets.Tests.EditMode
{
	/// <summary>
	/// Contains unit tests for the PlayerIntentArbiter class, verifying the correct arbitration of player intents based on
	/// various player states and input scenarios.
	/// </summary>
	/// <remarks>These tests ensure that PlayerIntentArbiter maintains expected behaviors, such as retaining
	/// traversal intents and dropping context-specific intents under certain conditions. Each test simulates different
	/// player states, including grounded and airborne modes, and validates the resulting set of intents after arbitration.
	/// The class is intended for use with NUnit and is sealed to prevent inheritance.</remarks>
	public sealed class PlayerIntentArbiterTests
	{
		[Test]
		public void Airborne_TertiaryPressed_DropsContextInteract_AndKeepsTraversalIntent()
		{
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				GroundedSubMode = PlayerGroundedSubMode.Standing,
			};

			var world = new PlayerWorldSnapshot { IsGrounded = false };

			var raw = new List<IPlayerIntent>
			{
				new MoveIntent(Vector2.One),
				new ContextInteractIntent(),
				new TertiaryPressedIntent(),
			};

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			result.ShouldContain(x => x is MoveIntent);
			result.ShouldContain(x => x is TertiaryPressedIntent);
			result.ShouldNotContain(x => x is ContextInteractIntent);
		}

		[Test]
		public void AlwaysKeepsMoveIntent()
		{
			var model = new PlayerModel();
			var world = new PlayerWorldSnapshot { IsGrounded = true };

			var raw = new List<IPlayerIntent> { new MoveIntent(Vector2.Zero) };

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			result.ShouldContain(x => x is MoveIntent);
		}

		[Test]
		public void Sprinting_TertiaryPressed_DropsContextInteract_AndKeepsTraversalIntent()
		{
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sprinting,
			};

			var world = new PlayerWorldSnapshot { IsGrounded = true };

			var raw = new List<IPlayerIntent>
			{
				new MoveIntent(Vector2.One),
				new ContextInteractIntent(),
				new TertiaryPressedIntent(),
			};

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			result.ShouldContain(x => x is MoveIntent);
			result.ShouldContain(x => x is TertiaryPressedIntent);
			result.ShouldNotContain(x => x is ContextInteractIntent);
		}

		[Test]
		public void UseSkillPresent_DropsTertiaryPressed_AndDropsContextInteract()
		{
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sprinting,
			};

			var world = new PlayerWorldSnapshot { IsGrounded = true };

			var raw = new List<IPlayerIntent>
			{
				new MoveIntent(Vector2.One),
				new TertiaryPressedIntent(),
				new ContextInteractIntent(),
				new UseSkillIntent(SkillBank.Secondary, 3),
			};

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			result.ShouldContain(x => x is MoveIntent);
			result.ShouldContain(x => x is UseSkillIntent);
			result.ShouldNotContain(x => x is TertiaryPressedIntent);
			result.ShouldNotContain(x => x is ContextInteractIntent);
		}
	}
}
