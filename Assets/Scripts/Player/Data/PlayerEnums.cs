namespace Assets.Scripts.Player.Data
{
	public enum PlayerSuperState
	{
		Grounded,
		Airborne,
		Climbing,
		Swimming,
	}

	public enum PlayerModifierPhase
	{
		Default,
		Modifier1,
		Modifier2,
	}

	public enum PlayerCombatStance
	{
		Holstered,
		Unholstered,
	}

	public enum GroundedSubState
	{
		Standing,
		Crouching,
		Sprinting,
		Sliding,
	}

	public enum StandingSubState
	{
		Idle,
		Walking,
		Jogging,
	}

	public enum CrouchingSubState
	{
		Idle,
		Sneaking,
	}

	public enum AirborneSubState
	{
		Ascending,
		Falling,
		Gliding,
	}
}
