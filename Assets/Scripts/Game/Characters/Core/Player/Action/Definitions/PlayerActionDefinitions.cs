using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public static class PlayerActionDefinitions
	{
		public static readonly PlayerActionDefinition LightAttack = new(
			PlayerActionId.LightAttack,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.LightAttack,
			new PlayerActionTiming(
				startupSeconds: 0.10f,
				activeSeconds: 0.08f,
				recoverySeconds: 0.22f
			),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.ActiveOrRecovery
			),
			new PlayerActionCancelPolicy(
				PlayerActionBufferWindow.RecoveryOnly,
				PlayerActionId.HeavyAttack
			)
		);

		public static readonly PlayerActionDefinition LightAttack2 = new(
			PlayerActionId.LightAttack2,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.LightAttack,
			new PlayerActionTiming(
				startupSeconds: 0.09f,
				activeSeconds: 0.08f,
				recoverySeconds: 0.20f
			),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.ActiveOrRecovery
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition LightAttack3 = new(
			PlayerActionId.LightAttack3,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.LightAttack,
			new PlayerActionTiming(
				startupSeconds: 0.12f,
				activeSeconds: 0.10f,
				recoverySeconds: 0.28f
			),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition HeavyAttack = new(
			PlayerActionId.HeavyAttack,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.HeavyAttack,
			new PlayerActionTiming(
				startupSeconds: 0.18f,
				activeSeconds: 0.10f,
				recoverySeconds: 0.35f
			),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.RecoveryOnly
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition Skill1 = new(
			PlayerActionId.Skill1,
			PlayerActionSource.Skill,
			PlayerActionCategory.Skill,
			AnimTrigger.Skill1,
			new PlayerActionTiming(
				startupSeconds: 0.15f,
				activeSeconds: 0.12f,
				recoverySeconds: 0.30f
			),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 20f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition Skill2 = new(
			PlayerActionId.Skill2,
			PlayerActionSource.Skill,
			PlayerActionCategory.Skill,
			AnimTrigger.Skill2,
			new PlayerActionTiming(
				startupSeconds: 0.15f,
				activeSeconds: 0.12f,
				recoverySeconds: 0.30f
			),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 25f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition Skill3 = new(
			PlayerActionId.Skill3,
			PlayerActionSource.Skill,
			PlayerActionCategory.Skill,
			AnimTrigger.Skill3,
			new PlayerActionTiming(
				startupSeconds: 0.15f,
				activeSeconds: 0.12f,
				recoverySeconds: 0.30f
			),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 30f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition ContextInteract = new(
			PlayerActionId.ContextInteract,
			PlayerActionSource.Context,
			PlayerActionCategory.Interact,
			AnimTrigger.ContextInteract,
			new PlayerActionTiming(
				startupSeconds: 0.05f,
				activeSeconds: 0.05f,
				recoverySeconds: 0.10f
			),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition ContextGrab = new(
			PlayerActionId.ContextGrab,
			PlayerActionSource.Context,
			PlayerActionCategory.Interact,
			AnimTrigger.ContextGrabOrFire,
			new PlayerActionTiming(
				startupSeconds: 0.06f,
				activeSeconds: 0.08f,
				recoverySeconds: 0.14f
			),
			new PlayerActionAvailability(requiresGrounded: false, allowWhileAirborne: true),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition FundamentalRangedPrimary = new(
			PlayerActionId.FundamentalRangedPrimary,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.ContextGrabOrFire,
			new PlayerActionTiming(
				startupSeconds: 0.06f,
				activeSeconds: 0.08f,
				recoverySeconds: 0.14f
			),
			new PlayerActionAvailability(requiresGrounded: false, allowWhileAirborne: true),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.RecoveryOnly
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition FundamentalBlockPrimary = new(
			PlayerActionId.FundamentalBlockPrimary,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.FundamentalBlockPrimary,
			new PlayerActionTiming(
				startupSeconds: 0.08f,
				activeSeconds: 0.10f,
				recoverySeconds: 0.18f
			),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.RecoveryOnly
			),
			PlayerActionCancelPolicy.None
		);

		private static readonly PlayerActionDefinition[] AllDefinitions =
		{
			LightAttack,
			LightAttack2,
			LightAttack3,
			HeavyAttack,
			Skill1,
			Skill2,
			Skill3,
			ContextInteract,
			ContextGrab,
			FundamentalRangedPrimary,
			FundamentalBlockPrimary,
		};

		private static readonly PlayerActionDefinitionRegistry DefaultRegistry = new(
			AllDefinitions
		);

		public static IReadOnlyList<PlayerActionDefinition> All => AllDefinitions;

		public static PlayerActionDefinitionRegistry CreateDefaultRegistry()
		{
			return DefaultRegistry;
		}

		public static PlayerActionDefinition Get(PlayerActionId id)
		{
			return DefaultRegistry.Get(id);
		}
	}
}
