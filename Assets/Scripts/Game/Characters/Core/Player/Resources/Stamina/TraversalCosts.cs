namespace Assets.Scripts.Game.Characters.Core.Player.Resources.Stamina
{
	public readonly struct TraversalCosts
	{
		public readonly float Leap;
		public readonly float KickOff;
		public readonly float Drop;
		public readonly float MinStaminaToEnterSprint;

		public TraversalCosts(float leap, float kickOff, float drop, float minStaminaToEnterSprint)
		{
			Leap = leap;
			KickOff = kickOff;
			Drop = drop;
			MinStaminaToEnterSprint = minStaminaToEnterSprint;
		}
	}
}
