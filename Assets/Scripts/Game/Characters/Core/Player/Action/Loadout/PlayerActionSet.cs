using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Model;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public sealed class PlayerActionSet
	{
		public PlayerActionBank NeutralBank { get; } = new();
		public PlayerActionBank AimBank { get; } = new();
		public PlayerActionBank BlockBank { get; } = new();
		public PlayerActionBank SpellReadyBank { get; } = new();

		public PlayerActionId SkillSlot1Id { get; set; } = PlayerActionId.Skill1;
		public PlayerActionId SkillSlot2Id { get; set; } = PlayerActionId.Skill2;
		public PlayerActionId SkillSlot3Id { get; set; } = PlayerActionId.Skill3;

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
