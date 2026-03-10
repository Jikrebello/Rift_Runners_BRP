using Assets.Scripts.Game.Characters.Core.Player.Model;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public sealed class PlayerActionSet
	{
		public PlayerActionBank NeutralBank { get; } = new();
		public PlayerActionBank AimBank { get; } = new();
		public PlayerActionBank BlockBank { get; } = new();
		public PlayerActionBank SpellReadyBank { get; } = new();

		public PlayerActionBank GetBank(PlayerCombatPosture posture)
		{
			return posture switch
			{
				PlayerCombatPosture.Aim => AimBank,
				PlayerCombatPosture.Block => BlockBank,
				PlayerCombatPosture.SpellReady => SpellReadyBank,
				_ => NeutralBank,
			};
		}
	}
}
