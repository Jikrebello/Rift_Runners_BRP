using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s;

namespace Assets.Scripts.Game.Characters.Core.Player.Traversal
{
	public sealed class GroundedState : ITraversalState
	{
		private GroundedTraversalConfig _config;

		public GroundedState()
			: this(GroundedTraversalConfig.Default) { }

		public GroundedState(GroundedTraversalConfig config)
		{
			_config = config;
		}

		public void SetConfig(GroundedTraversalConfig config) => _config = config;

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
					HandleToggleSprint(model, ResolveMovementProfile(model));
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
			var movementProfile = ResolveMovementProfile(model);
			EnforceMovementProfileConstraints(model, movementProfile);
			var effectiveMove = model.MoveInput * movementProfile.MoveInputMultiplier;

			outputs.Animation.AddBool(
				AnimBool.Crouching,
				model.GroundedSubMode == PlayerGroundedSubMode.Crouching
			);

			outputs.Animation.AddBool(
				AnimBool.Sprinting,
				model.GroundedSubMode == PlayerGroundedSubMode.Sprinting
			);

			var speedBlend = effectiveMove.Length();
			outputs.Animation.AddFloat(AnimFloat.Speed, speedBlend);

			outputs.Motor.DesiredMove = effectiveMove;

			if (model.WantsJumpThisFrame)
			{
				outputs.Motor.RequestJump = true;
				model.WantsJumpThisFrame = false;
			}
		}

		private static void HandleJumpPressed(PlayerModel model)
		{
			if (model.GroundedSubMode == PlayerGroundedSubMode.Sliding)
				return;

			model.WantsJumpThisFrame = true;
		}

		private static void HandleMove(PlayerModel model, MoveIntent intent)
		{
			model.MoveInput = intent.Direction;
		}

		private static void HandleTertiaryPressed(PlayerModel model)
		{
			if (model.GroundedSubMode == PlayerGroundedSubMode.Sprinting)
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

		private static void EnforceMovementProfileConstraints(
			PlayerModel model,
			GroundedMovementProfile movementProfile
		)
		{
			if (
				!movementProfile.AllowSprint
				&& model.GroundedSubMode == PlayerGroundedSubMode.Sprinting
			)
			{
				model.GroundedSubMode = PlayerGroundedSubMode.Standing;
			}
		}

		private static void HandleToggleSprint(
			PlayerModel model,
			GroundedMovementProfile movementProfile
		)
		{
			if (!movementProfile.AllowSprint)
				return;

			if (model.GroundedSubMode == PlayerGroundedSubMode.Sprinting)
			{
				model.GroundedSubMode = PlayerGroundedSubMode.Standing;
				return;
			}

			model.GroundedSubMode = PlayerGroundedSubMode.Sprinting;
			model.CombatStance = PlayerCombatStance.Holstered;
		}

		private GroundedMovementProfile ResolveMovementProfile(PlayerModel model)
		{
			return model.CombatPosture == PlayerCombatPosture.Block
				? _config.BlockProfile
				: _config.DefaultProfile;
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
