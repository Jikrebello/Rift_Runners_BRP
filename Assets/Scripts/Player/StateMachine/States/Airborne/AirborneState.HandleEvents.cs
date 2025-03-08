using Assets.Scripts.Player.Data;
using UnityEngine;

namespace Assets.Scripts.Player.StateMachine.States.Airborne
{
	public partial class AirborneState
	{
		private void SubscribeToEvents()
		{
			PlayerContext.PlayerInputEvents.MoveEvent += HandleMoveEvent;
			PlayerContext.PlayerInputEvents.JumpEvent += HandleJumpEvent;
			PlayerContext.PlayerInputEvents.JumpHeldEvent += HandleJumpHeldEvent;
			PlayerContext.PlayerInputEvents.JumpCancelledEvent += HandleJumpCancelledEvent;
			PlayerContext.PlayerInputEvents.SlideEvent += HandleSlideEvent;
		}

		private void UnsubscribeFromEvents()
		{
			PlayerContext.PlayerInputEvents.MoveEvent -= HandleMoveEvent;
			PlayerContext.PlayerInputEvents.JumpEvent -= HandleJumpEvent;
			PlayerContext.PlayerInputEvents.JumpHeldEvent -= HandleJumpHeldEvent;
			PlayerContext.PlayerInputEvents.JumpCancelledEvent -= HandleJumpCancelledEvent;
			PlayerContext.PlayerInputEvents.SlideEvent -= HandleSlideEvent;
		}

		private void HandleMoveEvent(Vector2 direction)
		{
			InputMoveDirection = direction;
		}

		private void HandleJumpEvent()
		{
			if (_canDoubleJump)
			{
				PlayerVelocity.y = _doubleJumpMagnitude;
				_canDoubleJump = false;
				_isLeap = false;
				_isSliding = false;
				CurrentSubState = AirborneSubState.Ascending;

				Debug.Log("Double Jump Triggered");
			}
		}

		private void HandleJumpHeldEvent()
		{
			_isHoldingJump = true;
			if (CurrentSubState == AirborneSubState.Falling && !_isGliding)
			{
				EnterGlideMode();
			}
		}

		private void HandleJumpCancelledEvent()
		{
			_isHoldingJump = false;
			if (_isGliding)
			{
				_isGliding = false;
				CurrentSubState = AirborneSubState.Falling;
				Debug.Log("Stopped Gliding");
			}
		}

		private void HandleSlideEvent()
		{
			if (!_canDoubleJump || _isGliding)
				return;

			Debug.Log("Slide Activated in Air! Cancelling Leap & Accelerating Fall.");
			if (_isLeap)
			{
				_isLeap = false;
				_isSliding = true;
			}

			_fallVelocity += _slideGravityMultiplier * Time.deltaTime;
			PlayerContext.PlayerAnimator.SetInteger(
				PlayerAnimationHashes.AirborneSubState,
				(int)AirborneSubState.Falling
			);
		}
	}
}
