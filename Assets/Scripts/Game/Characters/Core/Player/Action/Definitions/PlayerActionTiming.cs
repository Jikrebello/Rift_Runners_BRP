namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public readonly struct PlayerActionTiming
	{
		public readonly float StartupSeconds;
		public readonly float ActiveSeconds;
		public readonly float RecoverySeconds;

		public PlayerActionTiming(float startupSeconds, float activeSeconds, float recoverySeconds)
		{
			StartupSeconds = startupSeconds;
			ActiveSeconds = activeSeconds;
			RecoverySeconds = recoverySeconds;
		}
	}
}
