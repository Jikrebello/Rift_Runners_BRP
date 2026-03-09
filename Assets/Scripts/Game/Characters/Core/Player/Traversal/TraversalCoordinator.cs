using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Traversal
{
	public sealed class TraversalCoordinator
	{
		private readonly AirborneState _airborne;
		private readonly GroundedState _grounded;
		private readonly SlidingState _sliding;
		private readonly TraversalStateMachine _traversal;

		public TraversalCoordinator(
			TraversalStateMachine traversal,
			GroundedState grounded,
			SlidingState sliding,
			AirborneState airborne
		)
		{
			_traversal = traversal;
			_grounded = grounded;
			_sliding = sliding;
			_airborne = airborne;
		}

		public void ApplyTransitions(
			PlayerModel model,
			PlayerOutputs outputs,
			in PlayerWorldSnapshot world,
			IReadOnlyList<IPlayerIntent> intents
		)
		{
			if (TryStartSlide(model, outputs))
				return;

			if (TrySlidingLeap(model, outputs, intents))
				return;

			if (TrySlidingKickOff(model, outputs, intents))
				return;

			if (TryGroundedJumpToAirborne(model, outputs))
				return;

			if (TryAirborneLand(model, outputs, world))
				return;

			TryExitSlideToGrounded(model, outputs);
		}

		private static void ClearAirborneEntryTracking(PlayerModel model)
		{
			model.AirborneEnterKind = AirborneEnterKind.None;
			model.AirOptionConsumedThisAirborne = false;
		}

		private static bool ContainsIntent<T>(IReadOnlyList<IPlayerIntent> intents)
			where T : class, IPlayerIntent
		{
			for (int i = 0; i < intents.Count; i++)
			{
				if (intents[i] is T)
					return true;
			}

			return false;
		}

		private static void MarkAirborneEnteredByLeap(PlayerModel model)
		{
			model.AirborneEnterKind = AirborneEnterKind.Leap;
			model.AirOptionConsumedThisAirborne = false;
		}

		private static bool ShouldRollOnLanding(PlayerModel model)
		{
			return model.AirborneEnterKind == AirborneEnterKind.Leap
				&& !model.AirOptionConsumedThisAirborne;
		}

		private bool TryAirborneLand(
			PlayerModel model,
			PlayerOutputs outputs,
			in PlayerWorldSnapshot world
		)
		{
			if (model.TraversalMode != PlayerTraversalMode.Airborne)
				return false;

			if (!world.IsGrounded)
				return false;

			var shouldRoll = ShouldRollOnLanding(model);

			_traversal.TransitionTo(_grounded, model, outputs);

			if (shouldRoll)
				outputs.Animation.AddTrigger(AnimTrigger.Roll);

			ClearAirborneEntryTracking(model);

			return true;
		}

		private void TryExitSlideToGrounded(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.GroundedSubMode != PlayerGroundedSubMode.Sliding)
				return;

			if (!model.WantsExitSlideThisFrame)
				return;

			model.WantsExitSlideThisFrame = false;
			model.GroundedSubMode = PlayerGroundedSubMode.Standing;

			_traversal.TransitionTo(_grounded, model, outputs);
		}

		private bool TryGroundedJumpToAirborne(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.TraversalMode != PlayerTraversalMode.Grounded)
				return false;

			if (model.GroundedSubMode == PlayerGroundedSubMode.Sliding)
				return false;

			if (!outputs.Motor.RequestJump)
				return false;

			MarkAirborneEnteredByLeap(model);

			_traversal.TransitionTo(_airborne, model, outputs);
			return true;
		}

		private bool TrySlidingKickOff(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents
		)
		{
			if (model.GroundedSubMode != PlayerGroundedSubMode.Sliding)
				return false;

			if (!ContainsIntent<KickOffIntent>(intents))
				return false;

			model.GroundedSubMode = PlayerGroundedSubMode.Sprinting;

			_traversal.TransitionTo(_grounded, model, outputs);
			outputs.Animation.AddTrigger(AnimTrigger.KickOffJump);

			return true;
		}

		private bool TrySlidingLeap(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents
		)
		{
			if (model.GroundedSubMode != PlayerGroundedSubMode.Sliding)
				return false;

			if (!ContainsIntent<LeapIntent>(intents))
				return false;

			MarkAirborneEnteredByLeap(model);

			_traversal.TransitionTo(_airborne, model, outputs);
			return true;
		}

		private bool TryStartSlide(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.TraversalMode != PlayerTraversalMode.Grounded)
				return false;

			if (model.GroundedSubMode != PlayerGroundedSubMode.Sprinting)
				return false;

			if (!model.WantsSlideThisFrame)
				return false;

			model.WantsSlideThisFrame = false;

			_traversal.TransitionTo(_sliding, model, outputs);
			return true;
		}
	}
}
