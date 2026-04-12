using System;
using System.Collections.Generic;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout
{
	public sealed class PlayerCombatLoadoutCatalog
	{
		private readonly Dictionary<string, PlayerCombatLoadout> _loadouts;

		public PlayerCombatLoadoutCatalog(
			string defaultLoadoutId,
			IDictionary<string, PlayerCombatLoadout> loadouts
		)
		{
			if (string.IsNullOrWhiteSpace(defaultLoadoutId))
			{
				throw new ArgumentException(
					"Default loadout id is required.",
					nameof(defaultLoadoutId)
				);
			}

			if (loadouts == null)
				throw new ArgumentNullException(nameof(loadouts));

			_loadouts = new Dictionary<string, PlayerCombatLoadout>(StringComparer.OrdinalIgnoreCase);
			foreach (var pair in loadouts)
			{
				if (string.IsNullOrWhiteSpace(pair.Key))
					throw new ArgumentException("Loadout ids must be non-empty.", nameof(loadouts));

				if (pair.Value == null)
					throw new ArgumentException("Loadouts cannot be null.", nameof(loadouts));

				if (!_loadouts.TryAdd(pair.Key, PlayerCombatLoadouts.Clone(pair.Value)))
				{
					throw new ArgumentException(
						$"Duplicate loadout id detected for '{pair.Key}'.",
						nameof(loadouts)
					);
				}
			}

			if (!_loadouts.ContainsKey(defaultLoadoutId))
			{
				throw new ArgumentException(
					$"Default loadout id '{defaultLoadoutId}' was not found in the catalog.",
					nameof(defaultLoadoutId)
				);
			}

			DefaultLoadoutId = defaultLoadoutId;
		}

		public string DefaultLoadoutId { get; }

		public IReadOnlyCollection<string> LoadoutIds => _loadouts.Keys;

		public PlayerCombatLoadout GetDefaultLoadout()
		{
			return Get(DefaultLoadoutId);
		}

		public PlayerCombatLoadout Get(string id)
		{
			if (!TryGet(id, out var loadout))
				throw new KeyNotFoundException($"Combat loadout '{id}' was not found.");

			return loadout;
		}

		public bool TryGet(string id, out PlayerCombatLoadout loadout)
		{
			if (string.IsNullOrWhiteSpace(id))
			{
				loadout = new PlayerCombatLoadout();
				return false;
			}

			if (_loadouts.TryGetValue(id, out var stored))
			{
				loadout = PlayerCombatLoadouts.Clone(stored);
				return true;
			}

			loadout = new PlayerCombatLoadout();
			return false;
		}
	}
}
