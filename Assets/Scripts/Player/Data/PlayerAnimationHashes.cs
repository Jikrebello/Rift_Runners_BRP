using UnityEngine;

namespace Assets.Scripts.Player.Data
{
	public static class PlayerAnimationHashes
	{
		public static readonly int Speed = Animator.StringToHash("Speed");
		public static readonly int Crouching = Animator.StringToHash("Crouching");
		public static readonly int Roll = Animator.StringToHash("Roll");
		public static readonly int Sprinting = Animator.StringToHash("Sprinting");
		public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
		public static readonly int AirborneSubState = Animator.StringToHash("AirborneSubState");
		public static readonly int Sliding = Animator.StringToHash("Sliding");
		public static readonly int Holstered = Animator.StringToHash("Holstered");
		public static readonly int KickOffJump = Animator.StringToHash("Kick Off Jump");
	}
}
