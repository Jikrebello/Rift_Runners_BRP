namespace Assets.Scripts.Game.Characters.Core.Player.Model
{
	public enum AirborneEnterKind
	{
		None = 0,
		Leap = 1,
		// later: Knockback, Fall, JumpPad, etc.
	}

	public enum PlayerCombatPosture
	{
		None = 0,
		Aim = 1,
		Block = 2,
		SpellReady = 3,
	}

	public enum PlayerCombatStance
	{
		Holstered,
		Unholstered,
	}

	public enum PlayerExhaustionState
	{
		None = 0,
		DrainedMoving = 1,
		Recovering = 2,
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
		None,
		Active,
	}

	public enum SecondaryModifierMode
	{
		None,
		Active,
	}

	public enum UpperBodyMode
	{
		None = 0,
		Aim = 1,
		Block = 2,
		SpellReady = 3,
	}
}
