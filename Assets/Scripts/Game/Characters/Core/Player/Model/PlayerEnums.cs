namespace Assets.Scripts.Game.Characters.Core.Player.Model
{
	public enum AirborneEnterKind
	{
		None = 0,
		Leap = 1, // includes sprint jump and slide-hold leap
		// later: Knockback, Fall, JumpPad, etc.
	}

	public enum PlayerCombatStance
	{
		Holstered,
		Unholstered,
	}

	public enum PlayerGroundedSubMode
	{
		Standing,
		Crouching,
		Sprinting,
		Sliding,
	}

	public enum PlayerTraversalMode
	{
		Grounded,
		Airborne,
	}

	public enum PrimaryModifierMode
	{
		Default,
	}

	public enum SecondaryModifierMode
	{
		None,
		Active, // later split Aim/Block based on equipment
	}

	public enum UpperBodyMode
	{
		None = 0,
		Aim = 1,
		Block = 2,
	}
}
