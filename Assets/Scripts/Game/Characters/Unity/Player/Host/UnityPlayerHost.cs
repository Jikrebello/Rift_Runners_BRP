using Assets.Scripts.Game.Characters.Core.Player;
using Assets.Scripts.Game.Characters.Unity.Player.Anim;
using Assets.Scripts.Game.Characters.Unity.Player.Input;
using Assets.Scripts.Game.Characters.Unity.Player.Motor;
using Assets.Scripts.Game.Characters.Unity.Player.World;
using UnityEngine;

namespace Assets.Scripts.Game.Characters.Unity.Player.Host
{
	[RequireComponent(typeof(CharacterController))]
	[RequireComponent(typeof(Animator))]
	public sealed class UnityPlayerHost : MonoBehaviour
	{
		[SerializeField]
		private PlayerInputReader _inputReader;

		private PlayerPiece _player;
		private UnityInputAdapter _input;
		private UnityWorldSnapshotBuilder _world;
		private UnityMotorAdapter _motor;
		private UnityAnimatorAdapter _anim;

		private void Awake()
		{
			var cc = GetComponent<CharacterController>();
			var animator = GetComponent<Animator>();

			_player = new PlayerPiece();

			_input = new UnityInputAdapter(_inputReader);
			_world = new UnityWorldSnapshotBuilder(cc, transform);
			_motor = new UnityMotorAdapter(cc, transform);
			_anim = new UnityAnimatorAdapter(animator);
		}

		private void OnEnable()
		{
			_input = new UnityInputAdapter(_inputReader);
		}

		private void Update()
		{
			var input = _input.ConsumeSnapshot();
			var world = _world.Build(Time.deltaTime);

			var outputs = _player.Tick(input, world, Time.deltaTime);

			_anim.Apply(outputs.Animation);
			_motor.Apply(outputs.Motor, Time.deltaTime);
		}

		private void OnDisable()
		{
			_input?.Dispose();
			_input = null;
		}
	}
}
