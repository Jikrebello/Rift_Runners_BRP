using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public static class PlayerActionDefinitions
	{
		private const float SwordAdvanceSlashMoveMultiplier = 1.25f;

		public static readonly PlayerActionDefinition LightAttack = new(
			PlayerActionId.LightAttack,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.LightAttack,
			new PlayerActionTiming(0.10f, 0.08f, 0.22f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.ActiveOrRecovery
			),
			PlayerActionMotorProfile.None,
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
			new PlayerActionTiming(0.09f, 0.08f, 0.20f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.ActiveOrRecovery
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition LightAttack3 = new(
			PlayerActionId.LightAttack3,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.LightAttack,
			new PlayerActionTiming(0.12f, 0.10f, 0.28f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition HeavyAttack = new(
			PlayerActionId.HeavyAttack,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.HeavyAttack,
			new PlayerActionTiming(0.18f, 0.10f, 0.35f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.RecoveryOnly
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition SwordAdvanceSlash = new(
			PlayerActionId.SwordAdvanceSlash,
			PlayerActionSource.Skill,
			PlayerActionCategory.Skill,
			AnimTrigger.SwordAdvanceSlash,
			new PlayerActionTiming(0.16f, 0.10f, 0.32f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 25f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			new PlayerActionMotorProfile(
				PlayerActionMotorMode.MoveInputAdvance,
				PlayerActionPhase.Active,
				SwordAdvanceSlashMoveMultiplier
			),
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition SwordSkillSecondary = new(
			PlayerActionId.SwordSkillSecondary,
			PlayerActionSource.Skill,
			PlayerActionCategory.Skill,
			AnimTrigger.SwordSkillSecondary,
			new PlayerActionTiming(0.17f, 0.12f, 0.32f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 25f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition SwordSkillTertiary = new(
			PlayerActionId.SwordSkillTertiary,
			PlayerActionSource.Skill,
			PlayerActionCategory.Skill,
			AnimTrigger.SwordSkillTertiary,
			new PlayerActionTiming(0.20f, 0.14f, 0.34f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 30f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition ShieldGuardBash = new(
			PlayerActionId.ShieldGuardBash,
			PlayerActionSource.Skill,
			PlayerActionCategory.Skill,
			AnimTrigger.ShieldGuardBash,
			new PlayerActionTiming(0.10f, 0.08f, 0.20f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 15f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionMotorProfile.None,
			new PlayerActionCancelPolicy(
				PlayerActionBufferWindow.RecoveryOnly,
				PlayerActionId.FundamentalBlockPrimary
			)
		);

		public static readonly PlayerActionDefinition ShieldSkillSecondary = new(
			PlayerActionId.ShieldSkillSecondary,
			PlayerActionSource.Skill,
			PlayerActionCategory.Skill,
			AnimTrigger.ShieldSkillSecondary,
			new PlayerActionTiming(0.16f, 0.12f, 0.28f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 25f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition ShieldSkillTertiary = new(
			PlayerActionId.ShieldSkillTertiary,
			PlayerActionSource.Skill,
			PlayerActionCategory.Skill,
			AnimTrigger.ShieldSkillTertiary,
			new PlayerActionTiming(0.18f, 0.14f, 0.30f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 30f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition ContextInteract = new(
			PlayerActionId.ContextInteract,
			PlayerActionSource.Context,
			PlayerActionCategory.Interact,
			AnimTrigger.ContextInteract,
			new PlayerActionTiming(0.05f, 0.05f, 0.10f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition ContextGrab = new(
			PlayerActionId.ContextGrab,
			PlayerActionSource.Context,
			PlayerActionCategory.Interact,
			AnimTrigger.ContextGrabOrFire,
			new PlayerActionTiming(0.06f, 0.08f, 0.14f),
			new PlayerActionAvailability(requiresGrounded: false, allowWhileAirborne: true),
			new PlayerActionExecutionPolicy(
				canBuffer: false,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.None
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition FundamentalRangedPrimary = new(
			PlayerActionId.FundamentalRangedPrimary,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.ContextGrabOrFire,
			new PlayerActionTiming(0.06f, 0.08f, 0.14f),
			new PlayerActionAvailability(requiresGrounded: false, allowWhileAirborne: true),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.RecoveryOnly
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		public static readonly PlayerActionDefinition FundamentalBlockPrimary = new(
			PlayerActionId.FundamentalBlockPrimary,
			PlayerActionSource.Fundamental,
			PlayerActionCategory.Attack,
			AnimTrigger.FundamentalBlockPrimary,
			new PlayerActionTiming(0.08f, 0.10f, 0.18f),
			new PlayerActionAvailability(requiresGrounded: true, allowWhileAirborne: false),
			new PlayerActionExecutionPolicy(
				canBuffer: true,
				staminaCost: 0f,
				bufferWindow: PlayerActionBufferWindow.RecoveryOnly
			),
			PlayerActionMotorProfile.None,
			PlayerActionCancelPolicy.None
		);

		private static readonly PlayerActionDefinition[] AllDefinitions =
		{
			LightAttack,
			LightAttack2,
			LightAttack3,
			HeavyAttack,
			SwordAdvanceSlash,
			SwordSkillSecondary,
			SwordSkillTertiary,
			ContextInteract,
			ContextGrab,
			FundamentalRangedPrimary,
			FundamentalBlockPrimary,
			ShieldGuardBash,
			ShieldSkillSecondary,
			ShieldSkillTertiary,
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
