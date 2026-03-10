namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public static class PlayerActionGrammar
	{
		public static PlayerActionId ResolveFollowUp(
			PlayerActionId currentActionId,
			PlayerActionId requestedActionId
		)
		{
			return (currentActionId, requestedActionId) switch
			{
				(PlayerActionId.LightAttack, PlayerActionId.LightAttack) =>
					PlayerActionId.LightAttack2,
				(PlayerActionId.LightAttack2, PlayerActionId.LightAttack) =>
					PlayerActionId.LightAttack3,

				// Basic aimed/ranged primary can repeat itself for now.
				(
					PlayerActionId.FundamentalRangedPrimary,
					PlayerActionId.FundamentalRangedPrimary
				) => PlayerActionId.FundamentalRangedPrimary,

				// Default: no special grammar, use requested action as-is.
				_ => requestedActionId,
			};
		}
	}
}
