namespace Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s
{
	public readonly struct GroundedTraversalConfig
	{
		public const float DefaultMoveInputMultiplier = 1f;
		public const float DefaultBlockedMoveInputMultiplier = 0.5f;
		public const float DefaultBlockDashMoveMultiplier = 2.25f;
		public const bool DefaultSprintAllowed = true;
		public const bool DefaultBlockedSprintAllowed = false;
		public const bool DefaultBlockDashRequiresMoveInput = true;

		public static GroundedTraversalConfig Default =>
			new(
				new GroundedMovementProfile(DefaultMoveInputMultiplier, DefaultSprintAllowed),
				new GroundedMovementProfile(
					DefaultBlockedMoveInputMultiplier,
					DefaultBlockedSprintAllowed
				),
				DefaultBlockDashMoveMultiplier,
				DefaultBlockDashRequiresMoveInput
			);

		public readonly GroundedMovementProfile DefaultProfile;
		public readonly GroundedMovementProfile BlockProfile;
		public readonly float BlockDashMoveMultiplier;
		public readonly bool BlockDashRequiresMoveInput;

		public GroundedTraversalConfig(
			GroundedMovementProfile defaultProfile,
			GroundedMovementProfile blockProfile,
			float blockDashMoveMultiplier,
			bool blockDashRequiresMoveInput
		)
		{
			DefaultProfile = defaultProfile;
			BlockProfile = blockProfile;
			BlockDashMoveMultiplier = blockDashMoveMultiplier;
			BlockDashRequiresMoveInput = blockDashRequiresMoveInput;
		}
	}
}
