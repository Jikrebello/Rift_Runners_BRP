using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s;

namespace Assets.Tests.EditMode
{
	internal sealed class TraversalFixture
	{
		public readonly AirborneState Airborne = new();
		public readonly TraversalCoordinator Coordinator;
		public readonly GroundedState Grounded = new();
		public readonly SlidingState Sliding;
		public readonly SlidingStateConfig SlidingConfig;
		public readonly TraversalActionIntentSynthesizer Synthesizer;
		public readonly TraversalStateMachine Traversal = new();

		public TraversalFixture()
		{
			SlidingConfig = new SlidingStateConfig(
				kickOffTapMaxSeconds: 0.18f,
				leapHoldMinSeconds: 0.22f,
				slideStopSpeed: 0.35f
			);

			Sliding = new SlidingState(SlidingConfig);
			Synthesizer = new TraversalActionIntentSynthesizer(SlidingConfig);

			Coordinator = new TraversalCoordinator(Traversal, Grounded, Sliding, Airborne);
		}
	}
}
