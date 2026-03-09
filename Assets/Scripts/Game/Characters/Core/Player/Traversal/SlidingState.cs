using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s;

namespace Assets.Scripts.Game.Characters.Core.Player.Traversal
{
	public sealed class SlidingState : ITraversalState
	{
		private SlidingStateConfig _cfg;

		public SlidingState(SlidingStateConfig cfg) => _cfg = cfg;

		public void SetConfig(SlidingStateConfig cfg) => _cfg = cfg;

		public void Enter(PlayerModel model, PlayerOutputs outputs)
		{
			model.TraversalMode = PlayerTraversalMode.Grounded;
			model.GroundedSubMode = PlayerGroundedSubMode.Sliding;
			model.JumpIsHeld = false;
			model.JumpHoldTime = 0f;

			outputs.Animation.AddBool(AnimBool.IsGrounded, true);
			outputs.Animation.AddBool(AnimBool.Sprinting, false);
			outputs.Animation.AddBool(AnimBool.Crouching, false);
			outputs.Animation.AddBool(AnimBool.Sliding, true);
		}

		public void Exit(PlayerModel model, PlayerOutputs outputs)
		{
			model.JumpIsHeld = false;
			model.JumpHoldTime = 0f;
			outputs.Animation.AddBool(AnimBool.Sliding, false);
		}

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
			outputs.Motor.DesiredMove = model.MoveInput;

			if (world.PlanarSpeed <= _cfg.SlideStopSpeed)
				model.WantsExitSlideThisFrame = true;
		}
	}
}
