using System.Numerics;

namespace Assets.Scripts.Game.Characters.Core.Player.Outputs
{
	public sealed class MotorCommands
	{
		public Vector2 DesiredMove { get; set; } = Vector2.Zero;
		public bool GlideHeld { get; set; }
		public bool RequestDropThisFrame { get; set; }
		public bool RequestJump { get; set; }

		public void Clear()
		{
			DesiredMove = Vector2.Zero;
			RequestJump = false;
			GlideHeld = false;
			RequestDropThisFrame = false;
		}
	}
}
