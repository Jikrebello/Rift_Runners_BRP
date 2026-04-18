#nullable disable
using System.Collections.Generic;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions.Config
{
	public sealed class PlayerActionDefinitionCatalogConfig
	{
		public List<PlayerActionDefinitionConfig> Actions { get; set; } =
			new List<PlayerActionDefinitionConfig>();
	}

	public sealed class PlayerActionDefinitionConfig
	{
		public string Id { get; set; } = string.Empty;
		public string Source { get; set; } = string.Empty;
		public string Category { get; set; } = string.Empty;
		public string AnimationTrigger { get; set; } = string.Empty;
		public PlayerActionTimingConfig Timing { get; set; } = new PlayerActionTimingConfig();
		public PlayerActionAvailabilityConfig Availability { get; set; } =
			new PlayerActionAvailabilityConfig();
		public PlayerActionExecutionPolicyConfig Execution { get; set; } =
			new PlayerActionExecutionPolicyConfig();
		public PlayerActionMotorProfileConfig Motor { get; set; }
		public PlayerActionCancelPolicyConfig CancelPolicy { get; set; }
	}

	public sealed class PlayerActionTimingConfig
	{
		public float StartupSeconds { get; set; }
		public float ActiveSeconds { get; set; }
		public float RecoverySeconds { get; set; }
	}

	public sealed class PlayerActionAvailabilityConfig
	{
		public bool RequiresGrounded { get; set; }
		public bool AllowWhileAirborne { get; set; }
	}

	public sealed class PlayerActionExecutionPolicyConfig
	{
		public bool CanBuffer { get; set; }
		public float StaminaCost { get; set; }
		public string BufferWindow { get; set; } = string.Empty;
	}

	public sealed class PlayerActionCancelPolicyConfig
	{
		public string Window { get; set; } = string.Empty;
		public List<string> AllowedTargetIds { get; set; } = new List<string>();
	}

	public sealed class PlayerActionMotorProfileConfig
	{
		public string Mode { get; set; } = string.Empty;
		public string Phase { get; set; } = string.Empty;
		public float MoveMultiplier { get; set; }
	}
}
