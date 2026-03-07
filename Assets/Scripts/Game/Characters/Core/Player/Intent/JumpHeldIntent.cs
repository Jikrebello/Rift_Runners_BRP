namespace Assets.Scripts.Game.Characters.Core.Player.Intent
{
	public sealed class JumpHeldIntent : IPlayerIntent
	{
		public JumpHeldIntent(bool isHeld) => IsHeld = isHeld;

		public bool IsHeld { get; }
	}
}
