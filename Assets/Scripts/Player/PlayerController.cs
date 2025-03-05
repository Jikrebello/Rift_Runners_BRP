using Assets.Scripts.Player;
using Assets.Scripts.Player.Data;
using Assets.Scripts.Player.Input;
using Assets.Scripts.Player.StateMachine;
using Assets.Scripts.Player.StateMachine.States.Grounded;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public PlayerInputReader _inputReader;
	private IPlayerInputEvents _inputEvents;
	private PlayerContext _playerContext;
	private PlayerStateMachine _stateMachine;

	void Start()
	{
		SetupPlayerContext();

		_stateMachine = new PlayerStateMachine(_playerContext);
		_playerContext.StateMachine = _stateMachine;

		_stateMachine.TransitionTo(new StandingState());
	}

	void Update()
	{
		_stateMachine.Update();
	}

	private void FixedUpdate()
	{
		_stateMachine.FixedUpdate();
	}

	private void SetupPlayerContext()
	{
		_inputEvents = _inputReader;

		_playerContext = new PlayerContext
		{
			GameObject = gameObject,
			PlayerTransform = gameObject.transform,
			PlayerAnimator = GetComponent<Animator>(),
			PlayerInputEvents = _inputEvents,
			StateMachine = _stateMachine,
			CharacterController = GetComponent<CharacterController>(),
			CurrentPhase = PlayerModifierPhase.Default,
		};
	}
}
