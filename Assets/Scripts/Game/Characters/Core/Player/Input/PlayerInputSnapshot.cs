using System.Numerics;

namespace Assets.Scripts.Game.Characters.Core.Player.Input
{
	public struct PlayerInputSnapshot
	{
		// Trigger buttons
		public ButtonState ContextGrabOrFire;

		// Face buttons
		public ButtonState Jump;

		public Vector2 Look;

		// Left and Right sticks, or WASD and mouse delta, etc.
		public Vector2 Move;

		public ButtonState Primary;
		public ButtonState PrimarySkillModifier;
		public ButtonState Secondary;

		// Shoulder buttons
		public ButtonState SecondarySkillModifier;

		public ButtonState Tertiary;

		// Stick inputs
		public ButtonState ToggleCrouch;

		public ButtonState ToggleSprint;
		public ButtonState ToggleWeaponStance;
	}
}
