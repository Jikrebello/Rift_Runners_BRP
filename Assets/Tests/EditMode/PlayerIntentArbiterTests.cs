using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class PlayerIntentArbiterTests
	{
		[Test]
		public void AlwaysKeepsMoveIntent()
		{
			var model = new PlayerModel();
			var world = new PlayerWorldSnapshot { IsGrounded = true };

			var raw = new List<IPlayerIntent> { new MoveIntent(Vector2.Zero) };

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			Assert.That(result.Any(x => x is MoveIntent), Is.True);
		}

		[Test]
		public void GroundedStanding_CombatTertiary_DropsTraversalTertiaryAndKeepsCombatMeaning()
		{
			var model = NewGroundedStandingModel();
			var world = new PlayerWorldSnapshot { IsGrounded = true };

			var raw = new List<IPlayerIntent>
			{
				new MoveIntent(Vector2.One),
				new CombatTertiaryPressedIntent(),
				new TertiaryPressedIntent(),
			};

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			Assert.That(result.Any(x => x is CombatTertiaryPressedIntent), Is.True);
			Assert.That(result.Any(x => x is TertiaryPressedIntent), Is.False);
		}

		[Test]
		public void Sprinting_TertiaryPressed_DropsCombatTertiaryAndKeepsTraversalMeaning()
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
				new CombatTertiaryPressedIntent(),
				new TertiaryPressedIntent(),
				new ContextInteractIntent(),
			};

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			Assert.That(result.Any(x => x is CombatTertiaryPressedIntent), Is.False);
			Assert.That(result.Any(x => x is TertiaryPressedIntent), Is.True);
			Assert.That(result.Any(x => x is ContextInteractIntent), Is.False);
		}

		[Test]
		public void Airborne_TertiaryPressed_DropsCombatTertiaryAndKeepsTraversalMeaning()
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
				new CombatTertiaryPressedIntent(),
				new TertiaryPressedIntent(),
			};

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			Assert.That(result.Any(x => x is CombatTertiaryPressedIntent), Is.False);
			Assert.That(result.Any(x => x is TertiaryPressedIntent), Is.True);
		}

		[Test]
		public void GroundedStanding_DirectContextInteractIntent_IsPreserved()
		{
			var model = NewGroundedStandingModel();
			var world = new PlayerWorldSnapshot { IsGrounded = true };

			var raw = new List<IPlayerIntent>
			{
				new MoveIntent(Vector2.One),
				new ContextInteractIntent(),
				new TertiaryPressedIntent(),
			};

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			Assert.That(result.Any(x => x is ContextInteractIntent), Is.True);
			Assert.That(result.Any(x => x is TertiaryPressedIntent), Is.True);
		}

		[Test]
		public void Ordering_PutsCombatTraversalActionsBeforeMovement()
		{
			var model = NewGroundedStandingModel();
			var world = new PlayerWorldSnapshot { IsGrounded = true };

			var raw = new List<IPlayerIntent>
			{
				new MoveIntent(Vector2.One),
				new PrimaryPressedIntent(),
				new JumpPressedIntent(),
				new ToggleWeaponStanceIntent(),
			};

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			Assert.That(result.Count, Is.EqualTo(4));
			Assert.That(result[0] is ToggleWeaponStanceIntent, Is.True);
			Assert.That(result[1] is JumpPressedIntent, Is.True);
			Assert.That(result[2] is PrimaryPressedIntent, Is.True);
			Assert.That(result[3] is MoveIntent, Is.True);
		}

		[Test]
		public void UnknownIntent_IsPreserved_ByAppendUnknown()
		{
			var model = NewGroundedStandingModel();
			var world = new PlayerWorldSnapshot { IsGrounded = true };

			var unknown = new UnknownIntent();
			var raw = new List<IPlayerIntent> { new MoveIntent(Vector2.One), unknown };

			var result = PlayerIntentArbiter.Arbitrate(model, world, raw);

			Assert.That(result.Contains(unknown), Is.True);
			Assert.That(result.Last(), Is.SameAs(unknown));
		}

		private static PlayerModel NewGroundedStandingModel()
		{
			return new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				GroundedSubMode = PlayerGroundedSubMode.Standing,
			};
		}

		private sealed class UnknownIntent : IPlayerIntent { }
	}
}
