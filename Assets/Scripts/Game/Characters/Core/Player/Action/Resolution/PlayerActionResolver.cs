using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Loadout;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Resolution
{
	public sealed class PlayerActionResolver
	{
		public bool TryResolve(
			PlayerModel model,
			IReadOnlyList<IPlayerIntent> intents,
			out ResolvedPlayerActionRequest request
		)
		{
			var bank = model.ActionSet.GetBank(model.CombatPosture);

			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];

				if (intent is LightAttackIntent)
					return TryResolveById(bank.LightAttackId, out request);

				if (intent is HeavyAttackIntent)
					return TryResolveById(bank.HeavyAttackId, out request);

				if (intent is ContextInteractIntent)
				{
					request = new ResolvedPlayerActionRequest(
						PlayerActionDefinitions.ContextInteract
					);
					return true;
				}

				if (intent is RightActionIntent)
					return TryResolveById(bank.RightActionId, out request);

				if (intent is UseSkillIntent skill)
					return TryResolveSkill(bank, skill.Slot, out request);
			}

			request = default;
			return false;
		}

		private static bool TryResolveSkill(
			PlayerActionBank bank,
			int slot,
			out ResolvedPlayerActionRequest request
		)
		{
			var id = slot switch
			{
				1 => bank.SkillSlot1Id,
				2 => bank.SkillSlot2Id,
				3 => bank.SkillSlot3Id,
				_ => PlayerActionId.None,
			};

			return TryResolveById(id, out request);
		}

		private static bool TryResolveById(
			PlayerActionId id,
			out ResolvedPlayerActionRequest request
		)
		{
			var action = PlayerActionDefinitions.Get(id);
			if (action.Id == PlayerActionId.None)
			{
				request = default;
				return false;
			}

			request = new ResolvedPlayerActionRequest(action);
			return true;
		}
	}
}
