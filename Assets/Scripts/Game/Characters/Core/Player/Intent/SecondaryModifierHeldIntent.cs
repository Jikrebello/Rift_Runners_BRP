namespace Assets.Scripts.Game.Characters.Core.Player.Intent
{
	public sealed class SecondaryModifierHeldIntent : IPlayerIntent
	{
		public bool IsHeld;

		public SecondaryModifierHeldIntent(bool isHeld)
		{
			IsHeld = isHeld;
		}
	}
}