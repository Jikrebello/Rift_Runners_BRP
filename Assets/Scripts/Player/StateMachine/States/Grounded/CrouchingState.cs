using System.Collections.Generic;
using Assets.Scripts.Player.Data;

namespace Assets.Scripts.Player.StateMachine.States.Grounded
{
	class CrouchingState : GroundedState
	{
		private CrouchingSubState _currentCrouchSubState = new();
		private float _blendValue;

		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);

			CurrentGroundedSubState = GroundedSubState.Crouching;

			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Crouching, true);
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
			_blendValue = InputMoveDirection.magnitude;

			PlayerContext.PlayerAnimator.SetFloat(PlayerAnimationHashes.Speed, _blendValue);

			switch (_blendValue)
			{
				case 0:
					_currentCrouchSubState = CrouchingSubState.Idle;
					break;

				case >= 0.1f:
					_currentCrouchSubState = CrouchingSubState.Sneaking;
					break;
			}
		}

		private void HandleStateTransitions()
		{
			if (CurrentGroundedSubState == GroundedSubState.Standing)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Crouching, false);
				PlayerContext.StateMachine.TransitionTo(new StandingState());
			}
		}
	}
}
