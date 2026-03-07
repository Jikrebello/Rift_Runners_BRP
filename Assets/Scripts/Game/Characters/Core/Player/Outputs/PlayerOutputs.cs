namespace Assets.Scripts.Game.Characters.Core.Player.Outputs
{
	public sealed class PlayerOutputs
	{
		public AnimationCommands Animation { get; } = new();
		public MotorCommands Motor { get; } = new();

		public void Clear()
		{
			Animation.Clear();
			Motor.Clear();
		}
	}
}