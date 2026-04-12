using System;
using Assets.Scripts.Game.Characters.Core.Player.Input;
using Assets.Scripts.Game.Characters.Unity.Shared.Math;
using UVector2 = UnityEngine.Vector2;

namespace Assets.Scripts.Game.Characters.Unity.Player.Input
{
	public sealed class UnityInputAdapter : IDisposable
	{
		private readonly IPlayerInputEvents _events;

		private UVector2 _move;
		private UVector2 _look;

		private ButtonState _faceSouth;
		private ButtonState _faceWest;
		private ButtonState _faceNorth;
		private ButtonState _faceEast;

		private ButtonState _leftModifier;
		private ButtonState _rightModifier;

		private ButtonState _leftAction;
		private ButtonState _rightAction;

		private bool _leftTogglePressed;
		private bool _rightTogglePressed;

		private readonly Action<UVector2> _onMove;
		private readonly Action<UVector2> _onLook;

		private readonly Action _onFaceSouthPressed;
		private readonly Action _onFaceSouthReleased;

		private readonly Action _onFaceWestPressed;
		private readonly Action _onFaceWestReleased;

		private readonly Action _onFaceNorthPressed;
		private readonly Action _onFaceNorthReleased;

		private readonly Action _onFaceEastPressed;
		private readonly Action _onFaceEastReleased;

		private readonly Action _onLeftModifierPressed;
		private readonly Action _onLeftModifierReleased;

		private readonly Action _onRightModifierPressed;
		private readonly Action _onRightModifierReleased;

		private readonly Action _onLeftActionPressed;
		private readonly Action _onLeftActionReleased;

		private readonly Action _onRightActionPressed;
		private readonly Action _onRightActionReleased;

		private readonly Action _onLeftTogglePressed;
		private readonly Action _onRightTogglePressed;

		private bool _disposed;

		public UnityInputAdapter(IPlayerInputEvents events)
		{
			_events = events;

			_onMove = v => _move = v;
			_onLook = v => _look = v;

			_onFaceSouthPressed = () => Press(ref _faceSouth);
			_onFaceSouthReleased = () => Release(ref _faceSouth);

			_onFaceWestPressed = () => Press(ref _faceWest);
			_onFaceWestReleased = () => Release(ref _faceWest);

			_onFaceNorthPressed = () => Press(ref _faceNorth);
			_onFaceNorthReleased = () => Release(ref _faceNorth);

			_onFaceEastPressed = () => Press(ref _faceEast);
			_onFaceEastReleased = () => Release(ref _faceEast);

			_onLeftModifierPressed = () => Press(ref _leftModifier);
			_onLeftModifierReleased = () => Release(ref _leftModifier);

			_onRightModifierPressed = () => Press(ref _rightModifier);
			_onRightModifierReleased = () => Release(ref _rightModifier);

			_onLeftActionPressed = () => Press(ref _leftAction);
			_onLeftActionReleased = () => Release(ref _leftAction);

			_onRightActionPressed = () => Press(ref _rightAction);
			_onRightActionReleased = () => Release(ref _rightAction);

			_onLeftTogglePressed = () => _leftTogglePressed = true;
			_onRightTogglePressed = () => _rightTogglePressed = true;

			Subscribe();
		}

		public PlayerInputSnapshot ConsumeSnapshot()
		{
			var snapshot = new PlayerInputSnapshot
			{
				Move = _move.ToNumerics(),
				Look = _look.ToNumerics(),

				Jump = _faceSouth,
				Primary = _faceWest,
				Secondary = _faceNorth,
				Tertiary = _faceEast,

				PrimarySkillModifier = _rightModifier,
				SecondarySkillModifier = _leftModifier,

				ContextGrabOrFire = _rightAction,

				ToggleWeaponStance = new ButtonState
				{
					PressedThisFrame = _leftAction.PressedThisFrame,
				},

				ToggleSprint = new ButtonState { PressedThisFrame = _leftTogglePressed },
				ToggleCrouch = new ButtonState { PressedThisFrame = _rightTogglePressed },
			};

			ClearEdges(ref _faceSouth);
			ClearEdges(ref _faceWest);
			ClearEdges(ref _faceNorth);
			ClearEdges(ref _faceEast);

			ClearEdges(ref _leftModifier);
			ClearEdges(ref _rightModifier);

			ClearEdges(ref _leftAction);
			ClearEdges(ref _rightAction);

			_leftTogglePressed = false;
			_rightTogglePressed = false;

			return snapshot;
		}

		public void Dispose()
		{
			if (_disposed)
				return;
			_disposed = true;
			Unsubscribe();
		}

		private void Subscribe()
		{
			_events.MoveEvent += _onMove;
			_events.LookEvent += _onLook;

			_events.FaceSouthPressed += _onFaceSouthPressed;
			_events.FaceSouthReleased += _onFaceSouthReleased;

			_events.FaceWestPressed += _onFaceWestPressed;
			_events.FaceWestReleased += _onFaceWestReleased;

			_events.FaceNorthPressed += _onFaceNorthPressed;
			_events.FaceNorthReleased += _onFaceNorthReleased;

			_events.FaceEastPressed += _onFaceEastPressed;
			_events.FaceEastReleased += _onFaceEastReleased;

			_events.LeftModifierPressed += _onLeftModifierPressed;
			_events.LeftModifierReleased += _onLeftModifierReleased;

			_events.RightModifierPressed += _onRightModifierPressed;
			_events.RightModifierReleased += _onRightModifierReleased;

			_events.LeftActionPressed += _onLeftActionPressed;
			_events.LeftActionReleased += _onLeftActionReleased;

			_events.RightActionPressed += _onRightActionPressed;
			_events.RightActionReleased += _onRightActionReleased;

			_events.LeftTogglePressed += _onLeftTogglePressed;
			_events.RightTogglePressed += _onRightTogglePressed;
		}

		private void Unsubscribe()
		{
			_events.MoveEvent -= _onMove;
			_events.LookEvent -= _onLook;

			_events.FaceSouthPressed -= _onFaceSouthPressed;
			_events.FaceSouthReleased -= _onFaceSouthReleased;

			_events.FaceWestPressed -= _onFaceWestPressed;
			_events.FaceWestReleased -= _onFaceWestReleased;

			_events.FaceNorthPressed -= _onFaceNorthPressed;
			_events.FaceNorthReleased -= _onFaceNorthReleased;

			_events.FaceEastPressed -= _onFaceEastPressed;
			_events.FaceEastReleased -= _onFaceEastReleased;

			_events.LeftModifierPressed -= _onLeftModifierPressed;
			_events.LeftModifierReleased -= _onLeftModifierReleased;

			_events.RightModifierPressed -= _onRightModifierPressed;
			_events.RightModifierReleased -= _onRightModifierReleased;

			_events.LeftActionPressed -= _onLeftActionPressed;
			_events.LeftActionReleased -= _onLeftActionReleased;

			_events.RightActionPressed -= _onRightActionPressed;
			_events.RightActionReleased -= _onRightActionReleased;

			_events.LeftTogglePressed -= _onLeftTogglePressed;
			_events.RightTogglePressed -= _onRightTogglePressed;
		}

		private static void Press(ref ButtonState b)
		{
			b.PressedThisFrame = true;
			b.Held = true;
		}

		private static void Release(ref ButtonState b)
		{
			b.ReleasedThisFrame = true;
			b.Held = false;
		}

		private static void ClearEdges(ref ButtonState b)
		{
			b.PressedThisFrame = false;
			b.ReleasedThisFrame = false;
		}
	}
}
