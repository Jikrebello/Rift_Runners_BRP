namespace Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s
{
	public readonly struct SlidingStateConfig
	{
		public readonly float KickOffTapMaxSeconds;
		public readonly float LeapHoldMinSeconds;
		public readonly float SlideStopSpeed;

		public SlidingStateConfig(
			float kickOffTapMaxSeconds,
			float leapHoldMinSeconds,
			float slideStopSpeed
		)
		{
			KickOffTapMaxSeconds = kickOffTapMaxSeconds;
			LeapHoldMinSeconds = leapHoldMinSeconds;
			SlideStopSpeed = slideStopSpeed;
		}
	}
}
