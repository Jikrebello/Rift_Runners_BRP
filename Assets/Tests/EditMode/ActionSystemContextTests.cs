using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class ActionSystemContextTests
	{
		[Test]
		public void LightAttack_WhenGrounded_StartsNormally()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.LightAttack),
				Is.True
			);
		}

		[Test]
		public void LightAttack_WhenAirborne_IsRejected()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Airborne };
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.None));
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.None));
			Assert.That(outputs.Animation.Triggers.Count, Is.EqualTo(0));
		}

		[Test]
		public void HeavyAttack_WhenGrounded_StartsNormally()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.HeavyAttack),
				Is.True
			);
		}

		[Test]
		public void HeavyAttack_WhenAirborne_IsRejected()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Airborne };
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.None));
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.None));
			Assert.That(outputs.Animation.Triggers.Count, Is.EqualTo(0));
		}

		[Test]
		public void ContextInteract_WhenGrounded_StartsNormally()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new ContextInteractIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.ContextInteract)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.ContextInteract),
				Is.True
			);
		}

		[Test]
		public void ContextInteract_WhenAirborne_IsRejected()
		{
			var system = NewSystem();
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Airborne };
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new ContextInteractIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.None));
			Assert.That(outputs.Animation.Triggers.Count, Is.EqualTo(0));
		}

		[Test]
		public void RightAction_WithoutModifiers_ResolvesToContextGrab()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.None,
				SecondaryMode = SecondaryModifierMode.None,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.RightActionId = PlayerActionId.ContextGrab;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.ContextGrab)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.ContextGrabOrFire),
				Is.True
			);
		}

		[Test]
		public void RightAction_WithPrimaryModifier_UsesPrimaryModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.None,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.PrimaryModifierBank.RightActionId = PlayerActionId.Skill3;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill3));
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.Skill3),
				Is.True
			);
		}

		[Test]
		public void RightAction_WithSecondaryModifier_ResolvesToFundamentalRangedPrimary()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.None,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.RightActionId =
				PlayerActionId.FundamentalRangedPrimary;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.FundamentalRangedPrimary)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.ContextGrabOrFire),
				Is.True
			);
		}

		[Test]
		public void RightAction_WithDualModifiers_UsesDualModifierBank()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.Active,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.DualModifierBank.RightActionId = PlayerActionId.Skill2;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill2));
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.Skill2),
				Is.True
			);
		}

		[Test]
		public void RightAction_WhenBaseBankMapsToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				PrimaryMode = PrimaryModifierMode.None,
				SecondaryMode = SecondaryModifierMode.None,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.RightActionId = PlayerActionId.None;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(outputs.Animation.Triggers.Count, Is.EqualTo(0));
		}

		[Test]
		public void RightAction_WhenSecondaryModifierBankMapsToNone_DoesNotStartAction()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.RightActionId = PlayerActionId.None;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(model.ActionRuntime.HasActiveAction, Is.False);
			Assert.That(outputs.Animation.Triggers.Count, Is.EqualTo(0));
		}

		[Test]
		public void CombatPosture_Block_WithoutSecondaryModifier_DoesNotAffectRightActionResolution()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.Block,
				PrimaryMode = PrimaryModifierMode.None,
				SecondaryMode = SecondaryModifierMode.None,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.RightActionId = PlayerActionId.ContextGrab;
			model.CombatLoadout.ActionSet.SecondaryModifierBank.RightActionId =
				PlayerActionId.FundamentalRangedPrimary;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.ContextGrab)
			);
		}

		[Test]
		public void CombatPosture_SpellReady_WithoutSecondaryModifier_DoesNotAffectRightActionResolution()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				CombatPosture = PlayerCombatPosture.SpellReady,
				PrimaryMode = PrimaryModifierMode.None,
				SecondaryMode = SecondaryModifierMode.None,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.RightActionId = PlayerActionId.ContextGrab;
			model.CombatLoadout.ActionSet.SecondaryModifierBank.RightActionId =
				PlayerActionId.FundamentalRangedPrimary;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.ContextGrab)
			);
		}

		[Test]
		public void FundamentalRangedPrimary_WhenAirborne_RemainsAllowed()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.SecondaryModifierBank.RightActionId =
				PlayerActionId.FundamentalRangedPrimary;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.FundamentalRangedPrimary)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
		}

		[Test]
		public void ContextGrab_WhenAirborne_RemainsAllowed()
		{
			var system = NewSystem();
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Airborne,
				PrimaryMode = PrimaryModifierMode.None,
				SecondaryMode = SecondaryModifierMode.None,
			};
			var outputs = new PlayerOutputs();

			model.CombatLoadout.ActionSet.BaseBank.RightActionId = PlayerActionId.ContextGrab;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.ContextGrab)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
		}

		private static ActionTestDriver NewSystem() => new();
	}
}
