using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Unity.Player.Anim;
using Assets.Scripts.Game.Characters.Unity.Player.Input;
using Assets.Scripts.Game.Characters.Unity.Player.Motor;
using Assets.Scripts.Game.Characters.Unity.Player.Motor.Config;
using Assets.Scripts.Game.Characters.Unity.Player.Traversal.Config;
using Assets.Scripts.Game.Characters.Unity.Player.World;
using Assets.Scripts.Game.Characters.Unity.Shared.Config;
using UnityEngine;

namespace Assets.Scripts.Game.Characters.Unity.Player.Host
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(Animator))]
	public sealed class UnityPlayerHost : MonoBehaviour
	{
		[SerializeField]
		private PlayerInputReader _inputReader;

		[SerializeField]
		private bool _enableDebugLogs = true;

		[SerializeField]
		private DebugLogLevel _minLogLevel = DebugLogLevel.Info;

		[SerializeField]
		private string[] _enabledLogCategories = Array.Empty<string>();

		private HashSet<string> _logCategories = new();

		private UnityPlayerDebugLogAdapter _debug;

		private PlayerPiece _player;
		private UnityInputAdapter _input;
		private UnityWorldSnapshotBuilder _world;
		private UnityMotorAdapter _motor;
		private UnityAnimatorAdapter _anim;

		private float _reloadCooldown;
		private const float ReloadIntervalSeconds = 0.25f;

		private PollingConfigReloader<PlayerMotorConfig> _motorCfgReloader;
		private PollingConfigReloader<PlayerTraversalConfig> _travCfgReloader;

		private void Awake()
		{
			var cc = GetComponent<CharacterController>();
			var animator = GetComponent<Animator>();

			_logCategories = UnityPlayerDebugLogAdapter.BuildLogCategorySet(_enabledLogCategories);

			_motorCfgReloader = new PollingConfigReloader<PlayerMotorConfig>(
				PlayerMotorConfigPaths.PlayerMotorConfigPath
			);

			_travCfgReloader = new PollingConfigReloader<PlayerTraversalConfig>(
				PlayerTraversalConfigPaths.PlayerTraversalConfigPath
			);

			var slidingCfg = PlayerTraversalConfigMapper.ToSlidingStateConfig(
				_travCfgReloader.Current
			);

			_player = new PlayerPiece(slidingCfg);
			_motor = new UnityMotorAdapter(cc, transform, _motorCfgReloader.Current);

			_anim = new UnityAnimatorAdapter(animator);
			_world = new UnityWorldSnapshotBuilder(cc, transform);

			_debug = new UnityPlayerDebugLogAdapter(
				this,
				_logCategories,
				_minLogLevel,
				maxPerFrame: 16
			);
		}

		private void OnEnable()
		{
			_reloadCooldown = 0f;

			if (_inputReader == null)
			{
				Debug.LogError($"{nameof(UnityPlayerHost)}: Input Reader is not assigned.", this);
				enabled = false;
				return;
			}

			_input = new UnityInputAdapter(_inputReader);
		}

		private void Update()
		{
			var dt = Time.deltaTime;

			HotReloadConfigs(dt);

			var input = _input.ConsumeSnapshot();
			var world = _world.Build(dt);

			var outputs = _player.Tick(input, world, dt);

			_anim.Apply(outputs.Animation);
			_motor.Apply(outputs.Motor, dt);

			if (_enableDebugLogs)
				_debug.Apply(outputs.Debug);
		}

		private void OnDisable()
		{
			_input?.Dispose();
			_input = null;
		}

		private void OnDestroy()
		{
			_input?.Dispose();
		}

		private void HotReloadConfigs(float dt)
		{
#if UNITY_EDITOR
			_reloadCooldown -= dt;
			if (_reloadCooldown > 0f)
				return;

			_reloadCooldown = ReloadIntervalSeconds;

			// Motor
			if (_motorCfgReloader.PollAndReloadIfChanged(out var motorCfg))
			{
				_motor.SetConfig(motorCfg);
			}

			// Traversal
			if (_travCfgReloader.PollAndReloadIfChanged(out var travCfg))
			{
				var slidingCfg = PlayerTraversalConfigMapper.ToSlidingStateConfig(travCfg);
				_player.SetSlidingConfig(slidingCfg);
			}
#endif
		}
	}
}
