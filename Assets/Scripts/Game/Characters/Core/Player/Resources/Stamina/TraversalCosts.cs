namespace Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina
{
	public readonly struct TraversalCosts
	{
		public readonly float Leap;
		public readonly float KickOff;
		public readonly float MinStaminaToEnterSprint;

		public TraversalCosts(float leap, float kickOff, float minStaminaToEnterSprint)
		{
			Leap = leap;
			KickOff = kickOff;
			MinStaminaToEnterSprint = minStaminaToEnterSprint;
		}
	}
}
