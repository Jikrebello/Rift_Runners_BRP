using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Unity.Player.Motor.Config;
using Assets.Scripts.Game.Characters.Unity.Shared.Math;
using Assets.Scripts.Game.SharedKernel.Config;
using UnityEngine;

namespace Assets.Scripts.Game.Characters.Unity.Player.Motor
{
	public sealed class UnityMotorAdapter
	{
		private readonly CharacterController _cc;
		private readonly Transform _transform;

		private PlayerMotorConfig _cfg;
		private float _verticalVelocity;

		// Optional: reload timer so you don't hit disk every frame
		private float _reloadCooldown;

		public UnityMotorAdapter(CharacterController cc, Transform transform)
		{
			_cc = cc;
			_transform = transform;

			_cfg = JsonFileConfigLoader.Load<PlayerMotorConfig>(PlayerMotorConfigPaths.PlayerMotor);
		}

		public void Apply(MotorCommands motor, float dt)
		{
			// OPTIONAL hot reload in Editor only (safe + cheap)
#if UNITY_EDITOR
			_reloadCooldown -= dt;
			if (_reloadCooldown <= 0f)
			{
				_reloadCooldown = 0.25f; // 4x/sec
				if (
					JsonFileConfigLoader.TryReload<PlayerMotorConfig>(
						PlayerMotorConfigPaths.PlayerMotor,
						out var cfg
					)
				)
					_cfg = cfg;
			}
#endif

			// planar move
			var planar = motor.DesiredMove.ToUnity();
			var moveWorld = _transform.forward * planar.y + _transform.right * planar.x;
			moveWorld *= _cfg.MoveSpeed;

			// grounded clamp
			if (_cc.isGrounded && _verticalVelocity < 0f)
				_verticalVelocity = -2f;

			// jump
			if (motor.RequestJump && _cc.isGrounded)
				_verticalVelocity = _cfg.JumpSpeed;

			// drop overrides glide
			if (motor.RequestDropThisFrame)
				_verticalVelocity = _cfg.DropVelocity;

			// gravity (glide modifies)
			var gravity = _cfg.Gravity;
			if (!_cc.isGrounded && motor.GlideHeld && !motor.RequestDropThisFrame)
				gravity *= _cfg.GlideGravityMultiplier;

			_verticalVelocity += gravity * dt;

			// glide fall clamp
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
