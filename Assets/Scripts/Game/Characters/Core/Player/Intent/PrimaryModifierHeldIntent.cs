namespace Assets.Scripts.Game.Characters.Core.Player.Intent
{
	public sealed class PrimaryModifierHeldIntent : IPlayerIntent
	{
		public bool IsHeld;

		public PrimaryModifierHeldIntent(bool isHeld)
		{
			IsHeld = isHeld;
		}
	}
}
