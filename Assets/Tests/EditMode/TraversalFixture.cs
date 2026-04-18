using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s;

namespace Assets.Tests.EditMode
{
	internal sealed class TraversalFixture
	{
		public readonly AirborneState Airborne = new();
		public readonly TraversalCoordinator Coordinator;
		public readonly GroundedTraversalConfig GroundedConfig;
		public readonly GroundedState Grounded;
		public readonly SlidingState Sliding;
		public readonly SlidingStateConfig SlidingConfig;
		public readonly TraversalActionIntentSynthesizer Synthesizer;
		public readonly TraversalStateMachine Traversal = new();

		public TraversalFixture()
			: this(GroundedTraversalConfig.Default) { }

		public TraversalFixture(GroundedTraversalConfig groundedConfig)
		{
			GroundedConfig = groundedConfig;
			SlidingConfig = new SlidingStateConfig(
				kickOffTapMaxSeconds: 0.18f,
				leapHoldMinSeconds: 0.22f,
				slideStopSpeed: 0.35f
			);

			Grounded = new GroundedState(GroundedConfig);
			Sliding = new SlidingState(SlidingConfig);
			Synthesizer = new TraversalActionIntentSynthesizer(SlidingConfig);

			Coordinator = new TraversalCoordinator(Traversal, Grounded, Sliding, Airborne);
		}
	}
}
