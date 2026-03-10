using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public readonly struct PlayerActionExecutionPolicy
	{
		public readonly bool CanBuffer;
		public readonly float StaminaCost;
		public readonly PlayerActionBufferWindow BufferWindow;

		public PlayerActionExecutionPolicy(
			bool canBuffer,
			float staminaCost,
			PlayerActionBufferWindow bufferWindow
		)
		{
			CanBuffer = canBuffer;
			StaminaCost = staminaCost;
			BufferWindow = bufferWindow;
		}

		public bool AllowsBufferFrom(PlayerActionPhase phase)
		{
			return BufferWindow switch
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
