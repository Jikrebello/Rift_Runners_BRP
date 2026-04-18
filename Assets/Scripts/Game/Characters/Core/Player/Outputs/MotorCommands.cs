using System.Numerics;

namespace Assets.Scripts.Game.Characters.Core.Player.Outputs
{
	public sealed class MotorCommands
	{
		public Vector2 ActionMove { get; set; } = Vector2.Zero;
		public Vector2 BlockDashDirection { get; set; } = Vector2.Zero;
		public Vector2 BlockDashMove { get; set; } = Vector2.Zero;
		public Vector2 DesiredMove { get; set; } = Vector2.Zero;
		public bool GlideHeld { get; set; }
		public bool RequestBlockDashThisFrame { get; set; }
		public bool RequestDropThisFrame { get; set; }
		public bool RequestJump { get; set; }

		public Vector2 ResolvePlanarMove()
		{
			if (RequestBlockDashThisFrame)
				return BlockDashMove;

			if (ActionMove != Vector2.Zero)
				return ActionMove;

			return DesiredMove;
		}

		public void Clear()
		{
			ActionMove = Vector2.Zero;
			BlockDashDirection = Vector2.Zero;
			BlockDashMove = Vector2.Zero;
			DesiredMove = Vector2.Zero;
			RequestBlockDashThisFrame = false;
			RequestJump = false;
			GlideHeld = false;
			RequestDropThisFrame = false;
		}
	}
}
