using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Resolution
{
	public readonly struct ResolvedPlayerActionRequest
	{
		public readonly PlayerActionDefinition Action;

		public ResolvedPlayerActionRequest(PlayerActionDefinition action)
		{
			Action = action;
		}
	}
}
