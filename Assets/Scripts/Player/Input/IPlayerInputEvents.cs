using System;
using UnityEngine;

namespace Assets.Scripts.Player.Input
{
	public interface IPlayerInputEvents
	{
		event Action<Vector2> MoveEvent;
		event Action<Vector2> LookEvent;
		event Action JumpEvent;
		event Action JumpHeldEvent;
		event Action JumpCancelledEvent;
		event Action AimEvent;
		event Action AimCancelledEvent;
		event Action ContextualGrabEvent;
		event Action ContextualGrabCancelledEvent;
		event Action ContextualInteractEvent;
		event Action ContextualInteractCancelledEvent;
		event Action DodgeEvent;
		event Action DodgeCancelledEvent;
		event Action FireEvent;
		event Action FireCancelledEvent;
		event Action HeavyAttackEvent;
		event Action HeavyAttackCancelledEvent;
		event Action ManualReloadEvent;
		event Action ManualReloadCancelledEvent;
		event Action PistolWhipEvent;
		event Action PistolWhipCancelledEvent;
		event Action PrimaryAttackEvent;
		event Action PrimaryAttackCancelledEvent;
		event Action PrimaryModifierEvent;
		event Action PrimaryModifierCancelledEvent;
		event Action SpecialAttack1Event;
		event Action SpecialAttack1CancelledEvent;
		event Action SpecialAttack2Event;
		event Action SpecialAttack2CancelledEvent;
		event Action SpecialAttack3Event;
		event Action SpecialAttack3CancelledEvent;
		event Action ToggleCrouchEvent;
		event Action ToggleSprintEvent;
		event Action ToggleWeaponStanceEvent;
		event Action SlideEvent;
		event Action SlideCancelledEvent;
		event Action KickJumpEvent;
		event Action KickJumpCancelledEvent;
	}
}
