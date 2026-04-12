using System;
using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public readonly struct PlayerActionCancelPolicy
	{
		private static readonly PlayerActionId[] NoTargets = Array.Empty<PlayerActionId>();

		public static readonly PlayerActionCancelPolicy None = new(PlayerActionBufferWindow.None);

		public readonly PlayerActionBufferWindow Window;
		public readonly PlayerActionId[] AllowedTargetIds;

		public PlayerActionCancelPolicy(
			PlayerActionBufferWindow window,
			params PlayerActionId[] allowedTargetIds
		)
		{
			Window = window;
			AllowedTargetIds = allowedTargetIds ?? NoTargets;
		}

		public bool AllowsCancelTo(PlayerActionId requestedId, PlayerActionPhase phase)
		{
			if (!Window.AllowsPhase(phase))
				return false;

			for (int i = 0; i < AllowedTargetIds.Length; i++)
			{
				if (AllowedTargetIds[i] == requestedId)
					return true;
			}

			return false;
		}
	}
}
