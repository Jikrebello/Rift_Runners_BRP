using Assets.Scripts.Player.Input;
using Assets.Scripts.Player.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public class PlayerContext
	{
		public GameObject GameObject { get; set; }
		public Transform PlayerTransform { get; set; }
		public IPlayerInputEvents PlayerInputEvents { get; set; }
		public CharacterController CharacterController { get; set; }
		public PlayerStateMachine StateMachine { get; set; }

		public PlayerSuperState CurrentSuperState { get; set; }
		public PlayerModifierPhase CurrentPhase { get; set; }
		public PlayerCombatStance CurrentCombatStance { get; set; }
	}
}
