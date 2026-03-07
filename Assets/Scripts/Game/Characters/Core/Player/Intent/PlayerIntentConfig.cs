namespace Assets.Scripts.Game.Characters.Core.Player.Intent
{
	public sealed class PlayerIntentConfig
	{
		public PlayerIntentConfig(bool emitModifierHeldTransitions = true)
		{
			EmitModifierHeldTransitions = emitModifierHeldTransitions;
		}

		public bool EmitModifierHeldTransitions { get; }
	}
}
