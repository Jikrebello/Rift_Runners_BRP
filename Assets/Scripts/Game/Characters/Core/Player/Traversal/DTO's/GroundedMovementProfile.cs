namespace Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s
{
	public readonly struct GroundedMovementProfile
	{
		public readonly float MoveInputMultiplier;
		public readonly bool AllowSprint;

		public GroundedMovementProfile(float moveInputMultiplier, bool allowSprint)
		{
			MoveInputMultiplier = moveInputMultiplier;
			AllowSprint = allowSprint;
		}
	}
}
