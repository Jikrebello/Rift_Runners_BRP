using System;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions
{
	public sealed class PlayerActionDefinitionRegistry
	{
		private readonly Dictionary<PlayerActionId, PlayerActionDefinition> _definitions;

		public PlayerActionDefinitionRegistry(IEnumerable<PlayerActionDefinition> definitions)
		{
			if (definitions == null)
				throw new ArgumentNullException(nameof(definitions));

			_definitions = new Dictionary<PlayerActionId, PlayerActionDefinition>();

			foreach (var definition in definitions)
			{
				if (definition.Id == PlayerActionId.None)
				{
					throw new ArgumentException(
						"Action registry cannot contain PlayerActionId.None."
					);
				}

				if (!_definitions.TryAdd(definition.Id, definition))
				{
					throw new ArgumentException(
						$"Duplicate action definition detected for '{definition.Id}'."
					);
				}
			}
		}

		public IReadOnlyCollection<PlayerActionDefinition> Definitions => _definitions.Values;

		public PlayerActionDefinition Get(PlayerActionId id)
		{
			return _definitions.TryGetValue(id, out PlayerActionDefinition definition)
				? definition
				: default;
		}

		public bool TryGet(PlayerActionId id, out PlayerActionDefinition definition)
		{
			return _definitions.TryGetValue(id, out definition);
		}
	}
}
