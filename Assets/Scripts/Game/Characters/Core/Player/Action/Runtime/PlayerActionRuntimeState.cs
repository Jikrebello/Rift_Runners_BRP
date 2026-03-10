using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Runtime
{
	public sealed class PlayerActionRuntimeState
	{
		public PlayerActionId CurrentActionId { get; set; } = PlayerActionId.None;
		public PlayerActionPhase CurrentPhase { get; set; } = PlayerActionPhase.None;
		public float PhaseElapsedSeconds { get; set; }

		public PlayerActionId BufferedRequestedActionId { get; set; } = PlayerActionId.None;

		public bool HasActiveAction => CurrentActionId != PlayerActionId.None;
		public bool HasBufferedAction => BufferedRequestedActionId != PlayerActionId.None;

		public void ClearCurrent()
		{
			CurrentActionId = PlayerActionId.None;
			CurrentPhase = PlayerActionPhase.None;
			PhaseElapsedSeconds = 0f;
		}

		public void ClearBuffered()
		{
			BufferedRequestedActionId = PlayerActionId.None;
		}
	}
}
