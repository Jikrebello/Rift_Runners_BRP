using System;

namespace Assets.Scripts.Game.Characters.Unity.Player.Motor.Config
{
	[Serializable]
	public sealed class PlayerMotorConfig
	{
		public float MoveSpeed;
		public float JumpSpeed;

		// Negative
		public float Gravity;

		public float GlideGravityMultiplier;
		public float GlideMaxFallSpeed;

		public float DropVelocity;
	}

	public static class PlayerMotorConfigPaths
	{
		public const string PlayerMotorConfigPath = "Configs/Player/player_motor.json";
	}
}
