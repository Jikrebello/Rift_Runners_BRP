using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Traversal
{
	public sealed class GroundedState : ITraversalState
	{
		public void Enter(PlayerModel model, PlayerOutputs outputs)
		{
			model.TraversalMode = PlayerTraversalMode.Grounded;
			outputs.Animation.AddBool(AnimBool.IsGrounded, true);
		}

		public void Exit(PlayerModel model, PlayerOutputs outputs) { }

		public void HandleIntents(
			PlayerModel model,
			PlayerOutputs outputs,
			IReadOnlyList<IPlayerIntent> intents
		)
		{
			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];

				if (intent is MoveIntent move)
				{
					HandleMove(model, move);
					continue;
				}

				if (intent is ToggleCrouchIntent)
				{
					HandleToggleCrouch(model);
					continue;
				}

				if (intent is ToggleSprintIntent)
				{
					HandleToggleSprint(model, outputs);
					continue;
				}

				if (intent is ToggleWeaponStanceIntent)
				{
					HandleToggleWeaponStance(model, outputs);
					continue;
				}

				if (intent is TertiaryPressedIntent)
				{
					HandleTertiaryPressed(model);
					continue;
				}

				if (intent is JumpPressedIntent)
				{
					HandleJumpPressed(model);
				}
			}
		}

		public void Tick(
			PlayerModel model,
			PlayerOutputs outputs,
			in PlayerWorldSnapshot world,
			float dt
		)
		{
			outputs.Animation.AddBool(
				AnimBool.Crouching,
				model.GroundedSubMode == PlayerGroundedSubMode.Crouching
			);

			outputs.Animation.AddBool(
				AnimBool.Sprinting,
				model.GroundedSubMode == PlayerGroundedSubMode.Sprinting
			);

			var speedBlend = model.MoveInput.Length();
			outputs.Animation.AddFloat(AnimFloat.Speed, speedBlend);

			outputs.Motor.DesiredMove = model.MoveInput;

			if (model.WantsJumpThisFrame)
			{
				outputs.Motor.RequestJump = true;
				model.WantsJumpThisFrame = false;
			}
		}

		private static void HandleJumpPressed(PlayerModel model)
		{
			if (model.GroundedSubMode == PlayerGroundedSubMode.Sliding || model.IsSliding)
				return;

			model.WantsJumpThisFrame = true;
		}

		private static void HandleMove(PlayerModel model, MoveIntent intent)
		{
			model.MoveInput = intent.Direction;
		}

		private static void HandleTertiaryPressed(PlayerModel model)
		{
			if (model.GroundedSubMode == PlayerGroundedSubMode.Sprinting && !model.IsSliding)
			{
				model.WantsSlideThisFrame = true;
			}
		}

		private static void HandleToggleCrouch(PlayerModel model)
		{
			if (model.GroundedSubMode == PlayerGroundedSubMode.Sprinting)
			{
				model.GroundedSubMode = PlayerGroundedSubMode.Standing;
				return;
			}

			model.GroundedSubMode =
				model.GroundedSubMode == PlayerGroundedSubMode.Crouching
					? PlayerGroundedSubMode.Standing
					: PlayerGroundedSubMode.Crouching;
		}

		private static void HandleToggleSprint(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.GroundedSubMode == PlayerGroundedSubMode.Sprinting)
			{
				model.GroundedSubMode = PlayerGroundedSubMode.Standing;
				outputs.Animation.AddBool(AnimBool.Sprinting, false);
				return;
			}

			model.GroundedSubMode = PlayerGroundedSubMode.Sprinting;
			model.CombatStance = PlayerCombatStance.Holstered;
		}

		private static void HandleToggleWeaponStance(PlayerModel model, PlayerOutputs outputs)
		{
			if (model.GroundedSubMode == PlayerGroundedSubMode.Sprinting)
				return;

			bool isHolstered = model.CombatStance == PlayerCombatStance.Holstered;

			model.CombatStance = isHolstered
				? PlayerCombatStance.Unholstered
				: PlayerCombatStance.Holstered;

			outputs.Animation.AddBool(AnimBool.Holstered, isHolstered);
		}
	}
}
