namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public readonly struct PlayerActionAvailability
	{
		public readonly bool RequiresGrounded;
		public readonly bool AllowWhileAirborne;

		public PlayerActionAvailability(bool requiresGrounded, bool allowWhileAirborne)
		{
			RequiresGrounded = requiresGrounded;
			AllowWhileAirborne = allowWhileAirborne;
		}
	}
}
