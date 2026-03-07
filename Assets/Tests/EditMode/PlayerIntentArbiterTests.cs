using System.Collections.Generic;
using System.Numerics;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using NUnit.Framework;
using Shouldly;

namespace Assets.Tests.EditMode
{
	public sealed class PlayerIntentArbiterTests
	{
		[Test]
		public void Sprinting_TertiaryPressed_DropsContextInteract_AndKeepsTraversalIntent()
		{
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sprinting,
				IsSliding = false,
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
		public void Airborne_TertiaryPressed_DropsContextInteract_AndKeepsTraversalIntent()
		{
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				GroundedSubMode = PlayerGroundedSubMode.Standing,
				IsSliding = false,
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
		public void UseSkillPresent_DropsTertiaryPressed_AndDropsContextInteract()
		{
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Sprinting,
				IsSliding = false,
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

		[Test]
		public void AlwaysKeepsMoveIntent()
		{
			var model = new PlayerModel();
			var world = new PlayerWorldSnapshot { IsGrounded = true };

			var raw = new List<IPlayerIntent> { new MoveIntent(Vector2.Zero) };

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			result.ShouldContain(x => x is MoveIntent);
		}
	}
}