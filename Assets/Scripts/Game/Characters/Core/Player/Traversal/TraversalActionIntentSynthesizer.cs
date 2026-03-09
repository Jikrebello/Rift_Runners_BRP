using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s;

namespace Assets.Scripts.Game.Characters.Core.Player.Traversal
{
	public sealed class TraversalActionIntentSynthesizer
	{
		private SlidingStateConfig _slidingCfg;

		public TraversalActionIntentSynthesizer(SlidingStateConfig slidingCfg)
		{
			_slidingCfg = slidingCfg;
		}

		public void SetSlidingConfig(SlidingStateConfig cfg) => _slidingCfg = cfg;

		public IReadOnlyList<IPlayerIntent> Synthesize(
			PlayerModel model,
			IReadOnlyList<IPlayerIntent> intents,
			float dt
		)
		{
			ResetSlideJumpTrackingIfNotSliding(model);

			var result = new List<IPlayerIntent>(intents.Count + 2);

			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];

				if (TrySynthesizeAirborneIntent(model, intent, result))
					continue;

				if (TrySynthesizeSlidingIntent(model, intent, result))
					continue;

				result.Add(intent);
			}

			TrySynthesizeSlidingHoldLeap(model, result, dt);

			return result;
		}

		private static void ResetSlideJumpTrackingIfNotSliding(PlayerModel model)
		{
			if (model.GroundedSubMode == PlayerGroundedSubMode.Sliding)
				return;

			model.JumpIsHeld = false;
			model.JumpHoldTime = 0f;
		}

		private static bool TrySynthesizeAirborneIntent(
			PlayerModel model,
			IPlayerIntent intent,
			List<IPlayerIntent> result
		)
		{
			if (model.TraversalMode != PlayerTraversalMode.Airborne)
				return false;

			if (intent is not JumpHeldIntent held)
				return false;

			result.Add(held.IsHeld ? new BeginGlideIntent() : new EndGlideIntent());
			return true;
		}

		private bool TrySynthesizeSlidingIntent(
			PlayerModel model,
			IPlayerIntent intent,
			List<IPlayerIntent> result
		)
		{
			if (model.GroundedSubMode != PlayerGroundedSubMode.Sliding)
				return false;

			if (intent is JumpPressedIntent)
			{
				BeginSlidingJumpHold(model);
				return true;
			}

			if (intent is JumpReleasedIntent)
			{
				TryEmitSlidingKickOff(model, result);
				EndSlidingJumpHold(model);
				return true;
			}

			return false;
		}

		private static void BeginSlidingJumpHold(PlayerModel model)
		{
			model.JumpIsHeld = true;
			model.JumpHoldTime = 0f;
		}

		private static void EndSlidingJumpHold(PlayerModel model)
		{
			model.JumpIsHeld = false;
			model.JumpHoldTime = 0f;
		}

		private void TryEmitSlidingKickOff(PlayerModel model, List<IPlayerIntent> result)
		{
			if (!model.JumpIsHeld)
				return;

			if (model.JumpHoldTime > _slidingCfg.KickOffTapMaxSeconds)
				return;

			result.Add(new KickOffIntent());
		}

		private void TrySynthesizeSlidingHoldLeap(
			PlayerModel model,
			List<IPlayerIntent> result,
			float dt
		)
		{
			if (model.GroundedSubMode != PlayerGroundedSubMode.Sliding)
				return;

			if (!model.JumpIsHeld)
				return;

			model.JumpHoldTime += dt;

			if (model.JumpHoldTime < _slidingCfg.LeapHoldMinSeconds)
				return;

			result.Add(new LeapIntent());
			EndSlidingJumpHold(model);
		}
	}
}
