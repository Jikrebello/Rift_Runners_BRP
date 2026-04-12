using System;
using UnityEngine;

namespace Assets.Scripts.Game.Characters.Unity.Player.Input
{
	public interface IPlayerInputEvents
	{
		event Action<Vector2> MoveEvent;

		event Action<Vector2> LookEvent;

		event Action FaceSouthPressed;

		event Action FaceSouthReleased;

		event Action FaceWestPressed;

		event Action FaceWestReleased;

		event Action FaceNorthPressed;

		event Action FaceNorthReleased;

		event Action FaceEastPressed;

		event Action FaceEastReleased;

		event Action LeftModifierPressed;

		event Action LeftModifierReleased;

		event Action RightModifierPressed;

		event Action RightModifierReleased;

		event Action LeftTogglePressed;

		event Action RightTogglePressed;

		event Action LeftActionPressed;

		event Action LeftActionReleased;

		event Action RightActionPressed;

		event Action RightActionReleased;
	}
}
