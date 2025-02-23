using UnityEngine;

namespace Assets.Scripts.Player
{
	public class PlayerContext
	{
		public GameObject GameObject { get; set; }
		public IPlayerInputEvents PlayerInputEvents { get; set; }
		public CharacterController CharacterController { get; set; }
		public PlayerStateMachine StateMachine { get; set; }
	}
}
