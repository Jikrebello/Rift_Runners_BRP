using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Game.SharedKernel.Config;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions.Config
{
	public static class PlayerActionDefinitionCatalogLoader
	{
		public const string DefaultCatalogPath = "Configs/Player/player_action_definitions.json";

		public static PlayerActionDefinitionRegistry LoadExisting(string path = DefaultCatalogPath)
		{
			ValidatePath(path, nameof(path));

			if (
				!JsonFileConfigLoader.TryReload<PlayerActionDefinitionCatalogConfig>(
					path,
					out PlayerActionDefinitionCatalogConfig config
				)
			)
			{
				throw new FileNotFoundException(
					$"Action definition catalog file was not found: '{path}'.",
					path
				);
			}

			if (
				!PlayerActionDefinitionCatalogBuilder.TryBuild(
					config,
					out PlayerActionDefinitionRegistry registry,
					out IReadOnlyList<string> errors
				)
			)
			{
				throw new InvalidDataException(
					$"Action definition catalog is invalid:{Environment.NewLine}- {string.Join(Environment.NewLine + "- ", errors)}"
				);
			}

			return registry;
		}

		public static PlayerActionDefinitionCatalogConfig CreateDefaultCatalog()
		{
			return PlayerActionDefinitionCatalogBuilder.CreateConfig(PlayerActionDefinitions.All);
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
