using Assets.Scripts;
using Assets.Scripts.Player;
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
		Debug.Log("In player controller");

		_stateMachine = new PlayerStateMachine(_playerContext);
		_playerContext.StateMachine = _stateMachine;

		_stateMachine.TransitionTo(new GroundedState());
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
			PlayerInputEvents = _inputEvents,
			StateMachine = _stateMachine,
			CharacterController = GetComponent<CharacterController>(),
		};
	}
}
