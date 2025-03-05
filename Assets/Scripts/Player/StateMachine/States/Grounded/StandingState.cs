using System.Collections.Generic;
using Assets.Scripts.Player.Data;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States.Grounded
{
	public class StandingState : GroundedState
	{
		private StandingSubState _currentStandingSubState = new();

		private bool _isRolling = false;
		private float _blendValue;

		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);

			CurrentGroundedSubState = GroundedSubState.Standing;

			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sprinting, false);
			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Crouching, false);

			HandleEntryParameters(parameters);
		}

		private void HandleEntryParameters(Dictionary<string, object> parameters)
		{
			if (
				parameters != null
				&& parameters.ContainsKey(PlayerConstants.STANDING_IDLE)
				&& (bool)parameters[PlayerConstants.STANDING_IDLE]
			)
			{
				_currentStandingSubState = StandingSubState.Idle;
			}
			else if (
				parameters != null
				&& parameters.ContainsKey(PlayerConstants.JOGGING)
				&& (bool)parameters[PlayerConstants.JOGGING]
			)
			{
				_currentStandingSubState = StandingSubState.Jogging;
			}

			if (
				parameters != null
				&& parameters.ContainsKey(PlayerConstants.FROM_LEAP)
				&& (bool)parameters[PlayerConstants.FROM_LEAP]
			)
			{
				_isRolling = true;
				Debug.Log("Entering Walking State with a Roll!");
			}
		}

		public override void Update()
		{
			base.Update();

			ProcessAnimations();

			HandleStateTransitions();
		}

		public override void FixedUpdate() { }

		public override void Exit()
		{
			base.Exit();
		}

		private void ProcessAnimations()
		{
			ProcessStandingAnimations();

			ProcessRollAnimation();
		}

		private void ProcessStandingAnimations()
		{
			_blendValue = InputMoveDirection.magnitude;

			PlayerContext.PlayerAnimator.SetFloat(PlayerAnimationHashes.Speed, _blendValue);

			switch (_blendValue)
			{
				case 0:
					_currentStandingSubState = StandingSubState.Idle;
					break;
				case > 0
				and < 0.5f:
					_currentStandingSubState = StandingSubState.Walking;
					break;
				case >= 0.5f:
					_currentStandingSubState = StandingSubState.Jogging;
					break;
			}
		}

		private void ProcessRollAnimation()
		{
			if (_isRolling)
			{
				_isRolling = false;
				PlayerContext.PlayerAnimator.SetTrigger(PlayerAnimationHashes.Roll);
				Debug.Log("Roll Complete, now walking normally.");
				return;
			}
		}

		private void HandleStateTransitions()
		{
			if (
				InputMoveDirection.magnitude > 0
				&& CurrentGroundedSubState == GroundedSubState.Sprinting
			)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sprinting, true);
				PlayerContext.StateMachine.TransitionTo(new SprintingState());
			}

			if (CurrentGroundedSubState == GroundedSubState.Crouching)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Crouching, true);
				PlayerContext.StateMachine.TransitionTo(new CrouchingState());
			}
		}
	}
}
