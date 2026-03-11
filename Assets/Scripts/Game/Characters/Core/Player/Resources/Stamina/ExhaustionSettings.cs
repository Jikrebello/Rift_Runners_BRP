namespace Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina
{
	public readonly struct ExhaustionSettings
	{
		public readonly float DrainedMoveInitialMultiplier;
		public readonly float DrainedMoveDecayPerSecond;
		public readonly float RecoveryDurationSeconds;
		public readonly float StopMoveThreshold;

		public ExhaustionSettings(
			float drainedMoveInitialMultiplier,
			float drainedMoveDecayPerSecond,
			float recoveryDurationSeconds,
			float stopMoveThreshold
		)
		{
			DrainedMoveInitialMultiplier = drainedMoveInitialMultiplier;
			DrainedMoveDecayPerSecond = drainedMoveDecayPerSecond;
			RecoveryDurationSeconds = recoveryDurationSeconds;
			StopMoveThreshold = stopMoveThreshold;
		}
	}
}
