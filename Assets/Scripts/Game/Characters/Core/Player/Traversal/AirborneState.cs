using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Traversal
{
	public sealed class AirborneState : ITraversalState
	{
		public void Enter(PlayerModel model, PlayerOutputs outputs)
		{
			model.TraversalMode = PlayerTraversalMode.Airborne;
			model.IsGliding = false;
			model.WantsDropThisFrame = false;

			outputs.Animation.AddBool(AnimBool.IsGrounded, false);
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

				if (intent is MoveIntent m)
				{
					model.MoveInput = m.Direction;
					continue;
				}

				if (intent is JumpHeldIntent held)
				{
					// Glide is “while held”
					model.IsGliding = held.IsHeld;
					if (held.IsHeld)
						model.AirOptionConsumedThisAirborne = true;

					continue;
				}

				if (intent is JumpPressedIntent)
				{
					HandleAirJump(model, outputs);
					continue;
				}

				if (intent is TertiaryPressedIntent)
				{
					// Circle while airborne => drop
					model.WantsDropThisFrame = true;
					model.AirOptionConsumedThisAirborne = true;
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
			// Air control
			outputs.Motor.DesiredMove = model.MoveInput;

			// Glide/drop outputs for motor adapter
			outputs.Motor.GlideHeld = model.IsGliding;

			if (model.WantsDropThisFrame)
			{
				outputs.Motor.RequestDropThisFrame = true;
				model.WantsDropThisFrame = false;
			}
		}

		private static void HandleAirJump(PlayerModel model, PlayerOutputs outputs)
		{
			// First jump handled on ground. Airborne JumpPressed => double jump (once).
			if (model.HasDoubleJumped)
				return;

			model.HasDoubleJumped = true;
			model.AirOptionConsumedThisAirborne = true;
			outputs.Motor.RequestJump = true;
		}
	}
}
