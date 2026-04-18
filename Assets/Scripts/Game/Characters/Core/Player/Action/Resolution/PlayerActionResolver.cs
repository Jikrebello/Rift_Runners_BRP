using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Loadout;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Resolution
{
	public sealed class PlayerActionResolver
	{
		private readonly PlayerActionDefinitionRegistry _definitions;

		public PlayerActionResolver()
			: this(PlayerActionDefinitions.CreateDefaultRegistry()) { }

		public PlayerActionResolver(PlayerActionDefinitionRegistry definitions)
		{
			_definitions = definitions;
		}

		public bool TryResolve(
			PlayerModel model,
			IReadOnlyList<IPlayerIntent> intents,
			out ResolvedPlayerActionRequest request
		)
		{
			var selector = ResolveSelector(model);
			var bank = model.CombatLoadout.ActionSet.GetBank(selector);

			for (int i = 0; i < intents.Count; i++)
			{
				var intent = intents[i];

				if (intent is PrimaryPressedIntent)
					return TryResolveById(bank.PrimaryFaceActionId, out request);

				if (intent is SecondaryPressedIntent)
					return TryResolveById(bank.SecondaryFaceActionId, out request);

				if (intent is CombatTertiaryPressedIntent)
					return TryResolveById(bank.TertiaryFaceActionId, out request);

				if (intent is LightAttackIntent)
					return TryResolveById(PlayerActionId.LightAttack, out request);

				if (intent is HeavyAttackIntent)
					return TryResolveById(PlayerActionId.HeavyAttack, out request);

				if (intent is ContextInteractIntent)
					return TryResolveById(PlayerActionId.ContextInteract, out request);

				if (intent is RightActionIntent)
					return TryResolveById(bank.RightActionId, out request);
			}

			request = default;
			return false;
		}

		private static PlayerActionBankSelector ResolveSelector(PlayerModel model)
		{
			bool primary = model.IsPrimaryModifierActive;
			bool secondary = model.IsSecondaryModifierActive;

			if (secondary)
				return PlayerActionBankSelector.SecondaryModifier;

			if (primary)
				return PlayerActionBankSelector.PrimaryModifier;

			return PlayerActionBankSelector.Base;
		}

		private bool TryResolveById(PlayerActionId id, out ResolvedPlayerActionRequest request)
		{
			var action = _definitions.Get(id);
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
