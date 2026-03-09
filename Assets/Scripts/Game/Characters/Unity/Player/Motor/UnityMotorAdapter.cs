using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Unity.Player.Motor.Config;
using Assets.Scripts.Game.Characters.Unity.Shared.Math;
using UnityEngine;

namespace Assets.Scripts.Game.Characters.Unity.Player.Motor
{
	public sealed class UnityMotorAdapter
	{
		private readonly CharacterController _cc;
		private readonly Transform _transform;

		private PlayerMotorConfig _cfg;

		private float _verticalVelocity;

		public UnityMotorAdapter(CharacterController cc, Transform transform, PlayerMotorConfig cfg)
		{
			_cc = cc;
			_transform = transform;
			_cfg = cfg;
		}

		public void SetConfig(PlayerMotorConfig cfg)
		{
			_cfg = cfg;
		}

		public void Apply(MotorCommands motor, float dt)
		{
			var planar = motor.DesiredMove.ToUnity();
			var moveWorld = _transform.forward * planar.y + _transform.right * planar.x;
			moveWorld *= _cfg.MoveSpeed;

			if (_cc.isGrounded && _verticalVelocity < 0f)
				_verticalVelocity = -2f;

			if (motor.RequestJump && _cc.isGrounded)
				_verticalVelocity = _cfg.JumpSpeed;

			if (motor.RequestDropThisFrame)
				_verticalVelocity = _cfg.DropVelocity;

			var gravity = _cfg.Gravity;
			if (!_cc.isGrounded && motor.GlideHeld && !motor.RequestDropThisFrame)
				gravity *= _cfg.GlideGravityMultiplier;

			_verticalVelocity += gravity * dt;

			if (
				!_cc.isGrounded
				&& motor.GlideHeld
				&& !motor.RequestDropThisFrame
				&& _verticalVelocity < _cfg.GlideMaxFallSpeed
			)
				_verticalVelocity = _cfg.GlideMaxFallSpeed;

			var velocity = new Vector3(moveWorld.x, _verticalVelocity, moveWorld.z);
			_cc.Move(velocity * dt);
		}
	}
}
