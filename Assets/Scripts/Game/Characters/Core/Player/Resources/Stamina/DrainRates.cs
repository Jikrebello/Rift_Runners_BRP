namespace Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina
{
	public readonly struct DrainRates
	{
		public readonly float SprintPerSecond;
		public readonly float GlidePerSecond;

		public DrainRates(float sprintPerSecond, float glidePerSecond)
		{
			SprintPerSecond = sprintPerSecond;
			GlidePerSecond = glidePerSecond;
		}
	}
}
