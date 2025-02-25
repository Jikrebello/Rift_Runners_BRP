namespace Assets.Scripts.Player.StateMachine
{
	public enum PlayerSuperStates
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

	public enum GroundedSubStates
	{
		Standing,
		Crouching,
		Sprinting,
	}

	public enum AirborneSubStates
	{
		Ascending,
		Falling,
	}
}
