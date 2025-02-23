using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public class GroundedState : IPlayerState
	{
		private Vector2 _inputMoveDirection;

		public PlayerContext PlayerContext { get; set; }

		public void Enter(Dictionary<string, object> parameters)
		{
			PlayerContext.PlayerInputEvents.MoveEvent += HandleMove;
		}

		public void Update()
		{
			Debug.Log($"Player Input Direction {_inputMoveDirection}");
		}

		public void FixedUpdate() { }

		public void Exit() { }

		private void HandleMove(Vector2 direction)
		{
			_inputMoveDirection = direction;
		}
	}
}
