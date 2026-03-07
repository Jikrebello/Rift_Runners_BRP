using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

namespace Assets.Scripts.Game.Characters.Unity.Player.Input
{
	[CreateAssetMenu(menuName = "Game/Input/Player Input Reader")]
	public sealed class PlayerInputReader
		: ScriptableObject,
			PlayerInputs.IPlayerInputActionMapActions,
			IPlayerInputEvents
	{
		private PlayerInputs _inputs;

		[SerializeField]
		private bool _debug = true;

		public event Action<Vector2> MoveEvent;

		public event Action<Vector2> LookEvent;

		public event Action FaceSouthPressed;

		public event Action FaceSouthReleased;

		public event Action FaceWestPressed;

		public event Action FaceWestReleased;

		public event Action FaceNorthPressed;

		public event Action FaceNorthReleased;

		public event Action FaceEastPressed;

		public event Action FaceEastReleased;

		public event Action LeftModifierPressed;

		public event Action LeftModifierReleased;

		public event Action RightModifierPressed;

		public event Action RightModifierReleased;

		public event Action LeftTogglePressed;

		public event Action RightTogglePressed;

		public event Action LeftActionPressed;

		public event Action LeftActionReleased;

		public event Action RightActionPressed;

		public event Action RightActionReleased;

		private void OnEnable()
		{
			if (_inputs == null)
			{
				_inputs = new PlayerInputs();
				_inputs.PlayerInputActionMap.SetCallbacks(this);
			}

			_inputs.PlayerInputActionMap.Enable();
		}

		private void OnDisable()
		{
			_inputs?.PlayerInputActionMap.Disable();
		}

		public void OnMove(InputAction.CallbackContext context)
		{
			var v = context.ReadValue<Vector2>();
			LogValue("Move", context.phase, v);
			MoveEvent?.Invoke(v);
		}

		public void OnLook(InputAction.CallbackContext context)
		{
			var v = context.ReadValue<Vector2>();
			LogValue("Look", context.phase, v);
			LookEvent?.Invoke(v);
		}

		public void OnFaceSouth(InputAction.CallbackContext context)
		{
			LogButton("FaceSouth", context.phase);
			RaiseButton(context, FaceSouthPressed, FaceSouthReleased);
		}

		public void OnFaceEast(InputAction.CallbackContext context)
		{
			LogButton("FaceEast", context.phase);
			RaiseButton(context, FaceEastPressed, FaceEastReleased);
		}

		public void OnFaceWest(InputAction.CallbackContext context)
		{
			LogButton("FaceWest", context.phase);
			RaiseButton(context, FaceWestPressed, FaceWestReleased);
		}

		public void OnFaceNorth(InputAction.CallbackContext context)
		{
			LogButton("FaceNorth", context.phase);
			RaiseButton(context, FaceNorthPressed, FaceNorthReleased);
		}

		public void OnLeftModifier(InputAction.CallbackContext context)
		{
			LogButton("LeftModifier", context.phase);
			RaiseButton(context, LeftModifierPressed, LeftModifierReleased);
		}

		public void OnRightModifier(InputAction.CallbackContext context)
		{
			LogButton("RightModifier", context.phase);
			RaiseButton(context, RightModifierPressed, RightModifierReleased);
		}

		public void OnLeftToggle(InputAction.CallbackContext context)
		{
			LogButton("LeftToggle", context.phase);

			if (context.performed)
				LeftTogglePressed?.Invoke();
		}

		public void OnRightToggle(InputAction.CallbackContext context)
		{
			LogButton("RightToggle", context.phase);

			if (context.performed)
				RightTogglePressed?.Invoke();
		}

		public void OnLeftAction(InputAction.CallbackContext context)
		{
			LogButton("LeftAction", context.phase);
			RaiseButton(context, LeftActionPressed, LeftActionReleased);
		}

		public void OnRightAction(InputAction.CallbackContext context)
		{
			LogButton("RightAction", context.phase);
			RaiseButton(context, RightActionPressed, RightActionReleased);
		}

		private static void RaiseButton(
			InputAction.CallbackContext context,
			Action pressed,
			Action released
		)
		{
			if (context.performed)
			{
				pressed?.Invoke();
				return;
			}

			if (context.canceled)
				released?.Invoke();
		}

		private void LogButton(string name, InputActionPhase phase)
		{
			if (!_debug)
				return;
			Debug.Log($"{name} | phase={phase}");
		}

		private void LogValue(string name, InputActionPhase phase, Vector2 value)
		{
			if (!_debug)
				return;
			Debug.Log($"{name} | phase={phase} | value={value}");
		}
	}
}