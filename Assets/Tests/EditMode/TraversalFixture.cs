using Assets.Scripts.Game.Characters.Core.Player.Traversal;

namespace Assets.Tests.EditMode
{
	internal sealed class TraversalFixture
	{
		public readonly TraversalStateMachine Traversal = new();
		public readonly GroundedState Grounded = new();
		public readonly SlidingState Sliding = new();
		public readonly AirborneState Airborne = new();
		public readonly TraversalCoordinator Coordinator;

		public TraversalFixture()
		{
			Coordinator = new TraversalCoordinator(Traversal, Grounded, Sliding, Airborne);
		}
	}
}
