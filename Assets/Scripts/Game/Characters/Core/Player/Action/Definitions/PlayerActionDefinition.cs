using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public readonly struct PlayerActionDefinition
	{
		public readonly PlayerActionId Id;
		public readonly PlayerActionSource Source;
		public readonly PlayerActionCategory Category;
		public readonly AnimTrigger AnimationTrigger;
		public readonly PlayerActionTiming Timing;
		public readonly PlayerActionAvailability Availability;
		public readonly PlayerActionExecutionPolicy Execution;

		public PlayerActionDefinition(
			PlayerActionId id,
			PlayerActionSource source,
			PlayerActionCategory category,
			AnimTrigger animationTrigger,
			PlayerActionTiming timing,
			PlayerActionAvailability availability,
			PlayerActionExecutionPolicy execution
		)
		{
			Id = id;
			Source = source;
			Category = category;
			AnimationTrigger = animationTrigger;
			Timing = timing;
			Availability = availability;
			Execution = execution;
		}
	}
}
