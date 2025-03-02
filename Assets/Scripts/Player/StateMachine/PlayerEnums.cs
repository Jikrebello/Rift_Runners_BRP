namespace Assets.Scripts.Player.StateMachine
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
		Passive,
		Engaged,
	}

	public enum GroundedSubState
	{
		Standing,
		Crouching,
		Sprinting,
	}

	public enum AirborneSubState
	{
		Ascending,
		Falling,
		Gliding,
	}
}
