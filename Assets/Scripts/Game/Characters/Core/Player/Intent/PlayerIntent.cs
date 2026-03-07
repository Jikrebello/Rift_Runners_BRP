using System.Numerics;

namespace Assets.Scripts.Game.Characters.Core.Player.Intent
{
	public sealed class AimHeldIntent : IPlayerIntent
	{
		public bool IsHeld;

		public AimHeldIntent(bool held) => IsHeld = held;
	}

	public sealed class JumpPressedIntent : IPlayerIntent { }

	public sealed class JumpReleasedIntent : IPlayerIntent { }

	public sealed class MoveIntent : IPlayerIntent
	{
		public Vector2 Direction;

		public MoveIntent(Vector2 dir) => Direction = dir;
	}

	public sealed class ToggleCrouchIntent : IPlayerIntent { }

	public sealed class ToggleSprintIntent : IPlayerIntent { }

	public sealed class ToggleWeaponStanceIntent : IPlayerIntent { }
}
