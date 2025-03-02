using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputs;

namespace Assets.Scripts.Player.Input
{
	[CreateAssetMenu(menuName = "Player Input Reader")]
	public class PlayerInputReader
		: ScriptableObject,
			IPlayerInputActionMapActions,
			IPlayerInputEvents
	{
		PlayerInputs _playerInput;

		public bool IsDebug = false;

		public event Action<Vector2> MoveEvent;
		public event Action JumpEvent;
		public event Action JumpHeldEvent;
		public event Action JumpCancelledEvent;
		public event Action AimEvent;
		public event Action AimCancelledEvent;
		public event Action ContextualGrabEvent;
		public event Action ContextualGrabCancelledEvent;
		public event Action ContextualInteractEvent;
		public event Action ContextualInteractCancelledEvent;
		public event Action DodgeEvent;
		public event Action DodgeCancelledEvent;
		public event Action FireEvent;
		public event Action FireCancelledEvent;
		public event Action HeavyAttackEvent;
		public event Action HeavyAttackCancelledEvent;
		public event Action ManualReloadEvent;
		public event Action ManualReloadCancelledEvent;
		public event Action PistolWhipEvent;
		public event Action PistolWhipCancelledEvent;
		public event Action PrimaryAttackEvent;
		public event Action PrimaryAttackCancelledEvent;
		public event Action PrimaryModifierEvent;
		public event Action PrimaryModifierCancelledEvent;
		public event Action SpecialAttack1Event;
		public event Action SpecialAttack1CancelledEvent;
		public event Action SpecialAttack2Event;
		public event Action SpecialAttack2CancelledEvent;
		public event Action SpecialAttack3Event;
		public event Action SpecialAttack3CancelledEvent;
		public event Action ToggleCrouchEvent;
		public event Action ToggleSprintEvent;
		public event Action ToggleWeaponStanceEvent;

		private void OnEnable()
		{
			if (_playerInput == null)
			{
				_playerInput = new PlayerInputs();

				_playerInput.PlayerInputActionMap.SetCallbacks(this);

				SetPlayerInputMap();
			}
		}

		private void OnDisable()
		{
			_playerInput?.Disable();
		}

		private void SetPlayerInputMap()
		{
			_playerInput.PlayerInputActionMap.Enable();
		}

		public void OnMovement(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Movement\n" + $"Phase: {context.phase}, Value: {context.ReadValue<Vector2>()}"
				);
			}
			MoveEvent?.Invoke(context.ReadValue<Vector2>());
		}

		public void OnJump(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Jump\n" + $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Started)
			{
				JumpEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Performed)
			{
				JumpHeldEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				JumpCancelledEvent?.Invoke();
			}
		}

		public void OnAiming(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Aim\n" + $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				AimEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				AimCancelledEvent?.Invoke();
			}
		}

		public void OnContextualGrab(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Contextual Grab\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				ContextualGrabEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				ContextualGrabCancelledEvent?.Invoke();
			}
		}

		public void OnContextualInteract(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Contextual Interact\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				ContextualInteractEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				ContextualInteractCancelledEvent?.Invoke();
			}
		}

		public void OnDodge(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Dodge\n" + $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				DodgeEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				DodgeCancelledEvent?.Invoke();
			}
		}

		public void OnFire(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Fire\n" + $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				FireEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				FireCancelledEvent?.Invoke();
			}
		}

		public void OnHeavyAttack(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Heavy Attack\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				HeavyAttackEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				HeavyAttackCancelledEvent?.Invoke();
			}
		}

		public void OnManualReload(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Manual Reload\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				ManualReloadEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				ManualReloadCancelledEvent?.Invoke();
			}
		}

		public void OnPistolWhip(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Pistol Whip\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				PistolWhipEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				PistolWhipCancelledEvent?.Invoke();
			}
		}

		public void OnPrimaryAttack(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Primary Attack\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				PrimaryAttackEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				PrimaryAttackCancelledEvent?.Invoke();
			}
		}

		public void OnPrimaryModifier(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Primary Modifier\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				PrimaryModifierEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				PrimaryModifierCancelledEvent?.Invoke();
			}
		}

		public void OnSpecialAttack1(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Special Attack 1\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				SpecialAttack1Event?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				SpecialAttack1CancelledEvent?.Invoke();
			}
		}

		public void OnSpecialAttack2(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Special Attack 2\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				SpecialAttack2Event?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				SpecialAttack2CancelledEvent?.Invoke();
			}
		}

		public void OnSpecialAttack3(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Special Attack 3\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				SpecialAttack3Event?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				SpecialAttack3CancelledEvent?.Invoke();
			}
		}

		public void OnToggleCrouch(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Toggle Crouch\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				ToggleCrouchEvent?.Invoke();
			}
		}

		public void OnToggleSprint(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Toggle Sprint\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				ToggleSprintEvent?.Invoke();
			}
		}

		public void OnToggleWeaponStance(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Toggle Weapon Stance\n"
						+ $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				ToggleWeaponStanceEvent?.Invoke();
			}
		}
	}
}
