using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.SharedKernel.Config;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout.Config
{
	public static class PlayerCombatLoadoutCatalogLoader
	{
		public const string DefaultCatalogPath = "Configs/Player/player_combat_loadouts.json";

		public static PlayerCombatLoadoutCatalog LoadExisting(
			PlayerActionDefinitionRegistry actionDefinitions,
			string path = DefaultCatalogPath
		)
		{
			ValidatePath(path, nameof(path));

			if (actionDefinitions == null)
				throw new ArgumentNullException(nameof(actionDefinitions));

			if (
				!JsonFileConfigLoader.TryReload<PlayerCombatLoadoutCatalogConfig>(
					path,
					out PlayerCombatLoadoutCatalogConfig config
				)
			)
			{
				throw new FileNotFoundException(
					$"Combat loadout catalog file was not found: '{path}'.",
					path
				);
			}

			if (
				!PlayerCombatLoadoutCatalogBuilder.TryBuild(
					config,
					actionDefinitions,
					out PlayerCombatLoadoutCatalog catalog,
					out IReadOnlyList<string> errors
				)
			)
			{
				throw new InvalidDataException(
					$"Combat loadout catalog is invalid:{Environment.NewLine}- {string.Join(Environment.NewLine + "- ", errors)}"
				);
			}

			return catalog;
		}

		public static PlayerCombatLoadoutCatalogConfig CreateDefaultCatalog()
		{
			return PlayerCombatLoadoutCatalogBuilder.CreateConfig(
				PlayerCombatLoadouts.CreateDefaultCatalog()
			);
		}

		public static void SaveDefaultCatalog(string path = DefaultCatalogPath)
		{
			ValidatePath(path, nameof(path));
			JsonFileConfigLoader.Save(path, CreateDefaultCatalog());
		}

		private static void ValidatePath(string path, string paramName)
		{
			if (string.IsNullOrWhiteSpace(path))
				throw new ArgumentException("Path is required.", paramName);
		}
	}
}
