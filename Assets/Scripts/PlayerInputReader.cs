using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerInputs;

namespace Assets.Scripts
{
	[CreateAssetMenu(menuName = "Input Reader")]
	public class PlayerInputReader : ScriptableObject, IPlayerInputActionMapActions
	{
		PlayerInputs _playerInput;

		public bool IsDebug = false;

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

		public void SetPlayerInputMap()
		{
			_playerInput.PlayerInputActionMap.Enable();
		}

		public event Action<Vector2> MoveEvent;

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

		public event Action JumpEvent;
		public event Action JumpCancelledEvent;

		public void OnJump(InputAction.CallbackContext context)
		{
			if (IsDebug)
			{
				Debug.Log(
					$"Jump\n" + $"Phase: {context.phase}, Value: {context.ReadValue<float>()}"
				);
			}

			if (context.phase == InputActionPhase.Performed)
			{
				JumpEvent?.Invoke();
			}

			if (context.phase == InputActionPhase.Canceled)
			{
				JumpCancelledEvent?.Invoke();
			}
		}

		public event Action AimEvent;
		public event Action AimCancelledEvent;

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

		public event Action ContextualGrabEvent;
		public event Action ContextualGrabCancelledEvent;

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

		public event Action ContextualInteractEvent;
		public event Action ContextualInteractCancelledEvent;

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

		public event Action DodgeEvent;
		public event Action DodgeCancelledEvent;

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

		public event Action FireEvent;
		public event Action FireCancelledEvent;

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

		public event Action HeavyAttackEvent;
		public event Action HeavyAttackCancelledEvent;

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

		public event Action ManualReloadEvent;
		public event Action ManualReloadCancelledEvent;

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

		public event Action PistolWhipEvent;
		public event Action PistolWhipCancelledEvent;

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

		public event Action PrimaryAttackEvent;
		public event Action PrimaryAttackCancelledEvent;

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

		public event Action PrimaryModifierEvent;
		public event Action PrimaryModifierCancelledEvent;

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

		public event Action SpecialAttack1Event;
		public event Action SpecialAttack1CancelledEvent;

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

		public event Action SpecialAttack2Event;
		public event Action SpecialAttack2CancelledEvent;

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

		public event Action SpecialAttack3Event;
		public event Action SpecialAttack3CancelledEvent;

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

		public event Action ToggleCrouchEvent;
		public event Action ToggleCrouchCancelledEvent;

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

			if (context.phase == InputActionPhase.Canceled)
			{
				ToggleCrouchCancelledEvent?.Invoke();
			}
		}

		public event Action ToggleSprintEvent;
		public event Action ToggleSprintCancelledEvent;

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

			if (context.phase == InputActionPhase.Canceled)
			{
				ToggleSprintCancelledEvent?.Invoke();
			}
		}

		public event Action ToggleWeaponStanceEvent;
		public event Action ToggleWeaponStanceCancelledEvent;

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

			if (context.phase == InputActionPhase.Canceled)
			{
				ToggleWeaponStanceCancelledEvent?.Invoke();
			}
		}
	}
}
