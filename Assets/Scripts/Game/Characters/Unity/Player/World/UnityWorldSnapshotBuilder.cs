using Assets.Scripts.Game.Characters.Core.Player.Traversal;
using UnityEngine;

namespace Assets.Scripts.Game.Characters.Unity.Player.World
{
	public sealed class UnityWorldSnapshotBuilder
	{
		private readonly CharacterController _cc;
		private readonly Transform _transform;

		private Vector3 _prevPos;
		private bool _hasPrev;

		public UnityWorldSnapshotBuilder(CharacterController cc, Transform transform)
		{
			_cc = cc;
			_transform = transform;
		}

		public PlayerWorldSnapshot Build(float dt)
		{
			var pos = _transform.position;

			float planarSpeed = 0f;
			if (_hasPrev && dt > 0f)
			{
				var delta = (pos - _prevPos) / dt;
				delta.y = 0f;
				planarSpeed = delta.magnitude;
			}

			_prevPos = pos;
			_hasPrev = true;

			return new PlayerWorldSnapshot
			{
				IsGrounded = _cc.isGrounded,
				PlanarSpeed = planarSpeed,
			};
		}
	}
}
