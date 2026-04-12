using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public static class PlayerActionBufferWindowExtensions
	{
		public static bool AllowsPhase(
			this PlayerActionBufferWindow window,
			PlayerActionPhase phase
		)
		{
			return window switch
			{
				PlayerActionBufferWindow.None => false,
				PlayerActionBufferWindow.Any => true,
				PlayerActionBufferWindow.ActiveOnly => phase == PlayerActionPhase.Active,
				PlayerActionBufferWindow.RecoveryOnly => phase == PlayerActionPhase.Recovery,
				PlayerActionBufferWindow.ActiveOrRecovery => phase == PlayerActionPhase.Active
					|| phase == PlayerActionPhase.Recovery,
				_ => false,
			};
		}
	}
}
