using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
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
			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];

				if (intent is LightAttackIntent)
				{
					return TryResolveById(model.ActionSet.LightAttackId, out request);
				}

				if (intent is HeavyAttackIntent)
				{
					return TryResolveById(model.ActionSet.HeavyAttackId, out request);
				}

				if (intent is ContextInteractIntent)
				{
					request = new ResolvedPlayerActionRequest(
						PlayerActionDefinitions.ContextInteract
					);
					return true;
				}

				if (intent is RightActionIntent)
				{
					return TryResolveRightAction(model, out request);
				}

				if (intent is UseSkillIntent skill)
				{
					return TryResolveSkill(model, skill.Slot, out request);
				}
			}

			request = default;
			return false;
		}

		private static bool TryResolveRightAction(
			PlayerModel model,
			out ResolvedPlayerActionRequest request
		)
		{
			var id = model.CombatPosture switch
			{
				PlayerCombatPosture.Aim => model.ActionSet.SecondaryModeRightActionId,
				_ => model.ActionSet.NeutralRightActionId,
			};

			return TryResolveById(id, out request);
		}

		private static bool TryResolveSkill(
			PlayerModel model,
			int slot,
			out ResolvedPlayerActionRequest request
		)
		{
			var id = slot switch
			{
				1 => model.ActionSet.SkillSlot1Id,
				2 => model.ActionSet.SkillSlot2Id,
				3 => model.ActionSet.SkillSlot3Id,
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
