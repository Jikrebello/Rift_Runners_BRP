using System.Numerics;

namespace Assets.Scripts.Game.Characters.Core.Player.Model
{
	public sealed class PlayerModel
	{
		public AirborneEnterKind AirborneEnterKind { get; set; } = AirborneEnterKind.None;
		public bool AirOptionConsumedThisAirborne { get; set; }
		public PlayerCombatStance CombatStance { get; set; } = PlayerCombatStance.Holstered;
		public UpperBodyMode EquippedUpperBodyMode { get; set; } = UpperBodyMode.Aim;
		public PlayerGroundedSubMode GroundedSubMode { get; set; } = PlayerGroundedSubMode.Standing;
		public bool HasDoubleJumped { get; set; }
		public bool IsGliding { get; set; }
		public bool IsSecondaryModifierActive => SecondaryMode == SecondaryModifierMode.Active;
		public bool IsSliding { get; set; }
		public float JumpHoldTime { get; set; }
		public bool JumpIsHeld { get; set; }
		public Vector2 MoveInput { get; set; } = Vector2.Zero;
		public PrimaryModifierMode Phase { get; set; } = PrimaryModifierMode.Default;
		public SecondaryModifierMode SecondaryMode { get; set; } = SecondaryModifierMode.None;
		public PlayerTraversalMode TraversalMode { get; set; } = PlayerTraversalMode.Grounded;
		public bool WantsDropThisFrame { get; set; }
		public bool WantsJumpThisFrame { get; set; }
		public bool WantsKickOffThisFrame { get; set; }
		public bool WantsLeapThisFrame { get; set; }
		public bool WantsSlideThisFrame { get; set; }
	}
}
