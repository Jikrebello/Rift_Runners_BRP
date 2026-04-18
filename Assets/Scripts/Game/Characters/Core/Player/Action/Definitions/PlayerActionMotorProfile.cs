using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public enum PlayerActionMotorMode
	{
		None = 0,
		MoveInputAdvance = 1,
	}

	public readonly struct PlayerActionMotorProfile
	{
		public static readonly PlayerActionMotorProfile None = new(
			PlayerActionMotorMode.None,
			PlayerActionPhase.None,
			0f
		);

		public readonly PlayerActionMotorMode Mode;
		public readonly PlayerActionPhase Phase;
		public readonly float MoveMultiplier;

		public PlayerActionMotorProfile(
			PlayerActionMotorMode mode,
			PlayerActionPhase phase,
			float moveMultiplier
		)
		{
			Mode = mode;
			Phase = phase;
			MoveMultiplier = moveMultiplier;
		}

		public bool IsActiveDuring(PlayerActionPhase phase)
		{
			return Mode != PlayerActionMotorMode.None && Phase == phase;
		}
	}
}
