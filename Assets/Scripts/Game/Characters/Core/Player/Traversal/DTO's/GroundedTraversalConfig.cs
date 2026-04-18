namespace Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s
{
	public readonly struct GroundedTraversalConfig
	{
		public const float DefaultMoveInputMultiplier = 1f;
		public const float DefaultBlockedMoveInputMultiplier = 0.5f;
		public const bool DefaultSprintAllowed = true;
		public const bool DefaultBlockedSprintAllowed = false;

		public static GroundedTraversalConfig Default =>
			new(
				new GroundedMovementProfile(
					DefaultMoveInputMultiplier,
					DefaultSprintAllowed
				),
				new GroundedMovementProfile(
					DefaultBlockedMoveInputMultiplier,
					DefaultBlockedSprintAllowed
				)
			);

		public readonly GroundedMovementProfile DefaultProfile;
		public readonly GroundedMovementProfile BlockProfile;

		public GroundedTraversalConfig(
			GroundedMovementProfile defaultProfile,
			GroundedMovementProfile blockProfile
		)
		{
			DefaultProfile = defaultProfile;
			BlockProfile = blockProfile;
		}
	}
}
