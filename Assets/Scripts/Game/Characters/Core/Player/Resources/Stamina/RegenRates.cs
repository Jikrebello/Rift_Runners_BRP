namespace Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina
{
	public readonly struct RegenRates
	{
		public readonly float GroundedPerSecond;
		public readonly float SlidingPerSecond;

		public RegenRates(float groundedPerSecond, float slidingPerSecond)
		{
			GroundedPerSecond = groundedPerSecond;
			SlidingPerSecond = slidingPerSecond;
		}
	}
}
