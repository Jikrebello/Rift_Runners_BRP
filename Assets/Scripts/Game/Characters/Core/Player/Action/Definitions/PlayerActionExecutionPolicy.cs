namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public readonly struct PlayerActionExecutionPolicy
	{
		public readonly bool CanBuffer;
		public readonly float StaminaCost;

		public PlayerActionExecutionPolicy(bool canBuffer, float staminaCost)
		{
			CanBuffer = canBuffer;
			StaminaCost = staminaCost;
		}
	}
}
