namespace Assets.Scripts.Game.Characters.Core.Player.Intent
{
	public sealed class UseSkillIntent : IPlayerIntent
	{
		public UseSkillIntent(SkillBank bank, int slot)
		{
			Bank = bank;
			Slot = slot;
		}

		public SkillBank Bank { get; }
		public int Slot { get; }
	}
}
