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
			in PlayerWorldSnapshot world
		)
		{
			if (TryStartSlide(model, outputs))
				return;

			if (TrySlidingLeap(model, outputs))
				return;

			if (TrySlidingKickOff(model, outputs))
				return;

			if (TryGroundedJumpToAirborne(model, outputs))
				return;

			if (TryAirborneLand(model, outputs, world))
				return;

			TryExitSlideToGrounded(model, outputs);
		}

		/// <summary>
		/// Resets the airborne entry tracking state for the specified player model.
		/// </summary>
		/// <remarks>This method sets the player's airborne entry kind to none and marks the airborne option as not
		/// consumed, indicating that the player is no longer considered airborne.</remarks>
		/// <param name="model">The player model whose airborne entry tracking state is to be cleared.</param>
		private static void ClearAirborneEntryTracking(PlayerModel model)
		{
			model.AirborneEnterKind = AirborneEnterKind.None;
			model.AirOptionConsumedThisAirborne = false;
		}

		/// <summary>
		/// Marks the player as having entered an airborne state as a result of performing a leap.
		/// </summary>
		/// <remarks>This method sets the airborne entry kind to Leap and resets the air option consumption state for
		/// the specified player model.</remarks>
		/// <param name="model">The player model whose airborne entry state will be updated to reflect a leap action.</param>
		private static void MarkAirborneEnteredByLeap(PlayerModel model)
		{
			model.AirborneEnterKind = AirborneEnterKind.Leap;
			model.AirOptionConsumedThisAirborne = false;
		}

		/// <summary>
		/// Determines whether the player should perform a roll action upon landing based on their current airborne state.
		/// </summary>
		/// <remarks>Use this method to check if a roll should be triggered when the player lands, typically after a
		/// leap where the air option remains available.</remarks>
		/// <param name="model">The player model that represents the current state and actions of the player, used to evaluate landing behavior.</param>
		/// <returns>true if the player entered the airborne state by leaping and has not consumed their air option during this
		/// airborne phase; otherwise, false.</returns>
		private static bool ShouldRollOnLanding(PlayerModel model)
		{
			return model.AirborneEnterKind == AirborneEnterKind.Leap
				&& !model.AirOptionConsumedThisAirborne;
		}

		/// <summary>
		/// Attempts to transition the player from an airborne state to a grounded state if the player is currently airborne
		/// and detected as grounded.
		/// </summary>
		/// <remarks>If the player is eligible to roll upon landing, an animation trigger for rolling is added to the
		/// outputs. This method should be called when updating the player's traversal state to ensure proper handling of
		/// landing transitions.</remarks>
		/// <param name="model">The player model that contains the current traversal state and attributes of the player.</param>
		/// <param name="outputs">The outputs object used to trigger animations and other effects associated with the player's actions.</param>
		/// <param name="world">A snapshot of the player's world state, providing information such as whether the player is grounded.</param>
		/// <returns>true if the player successfully transitions to the grounded state; otherwise, false.</returns>
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

		/// <summary>
		/// Attempts to transition the player from a sliding state to a grounded standing state if the player is no longer
		/// sliding.
		/// </summary>
		/// <remarks>This method only performs the transition if the player was previously in the sliding submode and
		/// is no longer sliding. If the player is not in the sliding submode or is still sliding, no state change
		/// occurs.</remarks>
		/// <param name="model">The player model containing the current state and properties of the player character.</param>
		/// <param name="outputs">The outputs object that will be updated to reflect the player's new state after the transition.</param>
		private void TryExitSlideToGrounded(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.GroundedSubMode != PlayerGroundedSubMode.Sliding)
				return;

			if (model.IsSliding)
				return;

			model.GroundedSubMode = PlayerGroundedSubMode.Standing;
			_traversal.TransitionTo(_grounded, model, outputs);
		}

		/// <summary>
		/// Attempts to transition the player from a grounded state to an airborne state if a valid jump request is detected.
		/// </summary>
		/// <remarks>This method only processes jump requests when the player is in the grounded traversal mode and
		/// not sliding. If a valid jump request is present, the player's state is updated to airborne.</remarks>
		/// <param name="model">The player model containing the current traversal state and attributes of the player.</param>
		/// <param name="outputs">The outputs object that includes motor requests and other action-related information for the player.</param>
		/// <returns>true if the player successfully transitions to the airborne state; otherwise, false.</returns>
		private bool TryGroundedJumpToAirborne(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.TraversalMode != PlayerTraversalMode.Grounded)
				return false;

			// Sliding uses its own leap/kickoff rules
			if (model.GroundedSubMode == PlayerGroundedSubMode.Sliding)
				return false;

			// Motor request was emitted by GroundedState
			if (!outputs.Motor.RequestJump)
				return false;

			MarkAirborneEnteredByLeap(model);

			outputs.Motor.RequestJump = false;
			_traversal.TransitionTo(_airborne, model, outputs);
			return true;
		}

		/// <summary>
		/// Attempts to initiate a kick-off action while the player is sliding.
		/// </summary>
		/// <remarks>If the kick-off is performed, the player's grounded sub-mode is set to sprinting and the
		/// appropriate kick-off animation is triggered. This method should be called once per frame in response to player
		/// input.</remarks>
		/// <param name="model">The player model containing the current state and input flags for the player. Must not be null.</param>
		/// <param name="outputs">The outputs object used to record the results of the player's actions, such as animation triggers. Must not be
		/// null.</param>
		/// <returns>true if the sliding kick-off action was successfully initiated; otherwise, false.</returns>
		private bool TrySlidingKickOff(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.GroundedSubMode != PlayerGroundedSubMode.Sliding)
				return false;

			if (!model.WantsKickOffThisFrame)
				return false;

			model.WantsKickOffThisFrame = false;

			// Force sprint on return
			model.GroundedSubMode = PlayerGroundedSubMode.Sprinting;

			_traversal.TransitionTo(_grounded, model, outputs);

			outputs.Animation.AddTrigger(AnimTrigger.KickOffJump);

			return true;
		}

		/// <summary>
		/// Attempts to initiate a leap action for the player while sliding, based on the current input state.
		/// </summary>
		/// <remarks>This method resets the leap and kick-off requests for the current frame and transitions the
		/// player to an airborne state upon a successful leap.</remarks>
		/// <param name="model">The player model containing the current state and input requests for the player. Must not be null.</param>
		/// <param name="outputs">The outputs object used to record the results of the player's actions during the leap attempt. Must not be null.</param>
		/// <returns>true if the leap action is successfully initiated; otherwise, false.</returns>
		private bool TrySlidingLeap(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.GroundedSubMode != PlayerGroundedSubMode.Sliding)
				return false;

			if (!model.WantsLeapThisFrame)
				return false;

			model.WantsLeapThisFrame = false;
			model.WantsKickOffThisFrame = false;

			MarkAirborneEnteredByLeap(model);

			outputs.Motor.RequestJump = false;
			_traversal.TransitionTo(_airborne, model, outputs);
			return true;
		}

		/// <summary>
		/// Attempts to initiate a slide action for the player if the required conditions are met.
		/// </summary>
		/// <remarks>The slide action can only be initiated when the player is grounded, in a sprinting state, and has
		/// requested a slide during the current frame. After a successful attempt, the slide request flag is reset to prevent
		/// multiple slide initiations within the same frame.</remarks>
		/// <param name="model">The player model containing the current traversal state and input flags for the player.</param>
		/// <param name="outputs">The outputs object used to record the results of the player's actions during the slide attempt.</param>
		/// <returns>true if the slide action was successfully started; otherwise, false.</returns>
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
