using System;
using Assets.Scripts.Game.Characters.Core.Player.Traversal.DTO_s;

namespace Assets.Scripts.Game.Characters.Unity.Player.Traversal.Config
{
	[Serializable]
	public sealed class PlayerTraversalConfig
	{
		public float KickOffTapMaxSeconds = 0.18f;
		public float LeapHoldMinSeconds = 0.22f;
		public float SlideStopSpeed = 0.35f;
	}

	public static class PlayerTraversalConfigMapper
	{
		public static SlidingStateConfig ToSlidingStateConfig(PlayerTraversalConfig cfg)
		{
			return new SlidingStateConfig(
				cfg.KickOffTapMaxSeconds,
				cfg.LeapHoldMinSeconds,
				cfg.SlideStopSpeed
			);
		}
	}

	public static class PlayerTraversalConfigPaths
	{
		public const string PlayerTraversalConfigPath =
			"Configs/Player/player_traversal_config.json";
	}
}
