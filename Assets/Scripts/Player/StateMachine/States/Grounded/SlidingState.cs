using System;
using System.Collections.Generic;
using Assets.Scripts.Player.Data;
using Assets.Scripts.Player.StateMachine.States.Airborne;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States.Grounded
{
	public class SlidingState : GroundedState
	{
		private readonly float _momentumDecayRate = 3.0f;
		private readonly float _minimumMomentumThreshold = 0.1f;
		private readonly float _kickoffMomentumThreshold = 2.0f;
		private readonly float _kickoffJumpForce = 5.0f;

		private bool _isKickOffJump = false;

		private float _momentum;

		public override void Enter(Dictionary<string, object> parameters)
		{
			base.Enter(parameters);
			CurrentGroundedSubState = GroundedSubState.Sliding;

			// Get initial momentum from sprinting or airborne transition
			_momentum = parameters.GetFloat(PlayerConstants.INITIAL_MOMENTUM, 0.0f);

			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sliding, true);
		}

		public override void Update()
		{
			base.Update();
		}

		public override void FixedUpdate()
		{
			if (!PlayerContext.CharacterController.isGrounded)
			{
				PlayerContext.StateMachine.TransitionTo(new AirborneState());
				return;
			}

			// Decay momentum over time
			_momentum -= _momentumDecayRate * Time.deltaTime;
			_momentum = Mathf.Max(_momentum, 0);

			// 🚀 If player presses Jump and momentum is above threshold → Kickoff back into SprintState
			if (_isKickOffJump && _momentum > _kickoffMomentumThreshold)
			{
				KickoffJump();
				return;
			}

			// 🚀 If momentum runs out, transition into CrouchingState
			if (_momentum <= _minimumMomentumThreshold)
			{
				PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sliding, false);
				PlayerContext.StateMachine.TransitionTo(new CrouchingState());
				return;
			}

			// Move player forward based on momentum
			Vector3 movement = PlayerContext.PlayerTransform.forward * _momentum * Time.deltaTime;
			PlayerContext.CharacterController.Move(movement);
		}

		public override void Exit()
		{
			PlayerContext.PlayerInputEvents.MoveEvent -= HandleMoveEvent;
			PlayerContext.PlayerInputEvents.KickJumpEvent -= HandleJumpKickEvent;

			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sliding, false);
		}

		private void HandleMoveEvent(Vector2 vector)
		{
			throw new NotImplementedException();
		}

		private void HandleJumpKickEvent()
		{
			if (_isKickOffJump)
			{
				return;
			}
			_isKickOffJump = true;
		}

		private void KickoffJump()
		{
			Debug.Log("Kickoff Jump! Returning to Sprint");
			PlayerContext.PlayerAnimator.SetBool(PlayerAnimationHashes.Sliding, false);

			PlayerContext.StateMachine.TransitionTo(new SprintingState());
		}
	}
}
