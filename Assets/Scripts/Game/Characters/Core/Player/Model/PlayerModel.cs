using System.Numerics;
using Assets.Scripts.Game.Characters.Core.Player.Action.Loadout;
using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;

namespace Assets.Scripts.Game.Characters.Core.Player.Model
{
	public sealed class PlayerModel
	{
		public PlayerActionRuntimeState ActionRuntime { get; } = new();
		public PlayerActionSet ActionSet { get; } = PlayerActionSets.CreateDefault();

		public AirborneEnterKind AirborneEnterKind { get; set; } = AirborneEnterKind.None;
		public bool AirOptionConsumedThisAirborne { get; set; }
		public PlayerCombatPosture CombatPosture { get; set; } = PlayerCombatPosture.None;
		public PlayerCombatStance CombatStance { get; set; } = PlayerCombatStance.Holstered;
		public UpperBodyMode EquippedUpperBodyMode { get; set; } = UpperBodyMode.Aim;
		public PlayerGroundedSubMode GroundedSubMode { get; set; } = PlayerGroundedSubMode.Standing;
		public bool HasDoubleJumped { get; set; }
		public bool IsGliding { get; set; }
		public bool IsSecondaryModifierActive => SecondaryMode == SecondaryModifierMode.Active;
		public float JumpHoldTime { get; set; }
		public bool JumpIsHeld { get; set; }
		public float MaxStamina { get; set; }
		public Vector2 MoveInput { get; set; } = Vector2.Zero;
		public PrimaryModifierMode Phase { get; set; } = PrimaryModifierMode.Default;
		public SecondaryModifierMode SecondaryMode { get; set; } = SecondaryModifierMode.None;
		public float Stamina { get; set; }
		public PlayerTraversalMode TraversalMode { get; set; } = PlayerTraversalMode.Grounded;
		public bool WantsDropThisFrame { get; set; }
		public bool WantsExitSlideThisFrame { get; set; }
		public bool WantsJumpThisFrame { get; set; }
		public bool WantsSlideThisFrame { get; set; }
	}
}
