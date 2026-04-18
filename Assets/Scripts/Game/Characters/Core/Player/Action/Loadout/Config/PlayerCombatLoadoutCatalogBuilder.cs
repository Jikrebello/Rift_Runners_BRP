#nullable disable
using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Loadout.Config
{
	public static class PlayerCombatLoadoutCatalogBuilder
	{
		public static PlayerCombatLoadoutCatalogConfig CreateConfig(
			PlayerCombatLoadoutCatalog catalog
		)
		{
			if (catalog == null)
				throw new ArgumentNullException(nameof(catalog));

			var config = new PlayerCombatLoadoutCatalogConfig
			{
				DefaultLoadoutId = catalog.DefaultLoadoutId,
			};

			foreach (var loadoutId in catalog.LoadoutIds)
			{
				config.Loadouts.Add(CreateLoadoutConfig(loadoutId, catalog.Get(loadoutId)));
			}

			return config;
		}

		public static bool TryBuild(
			PlayerCombatLoadoutCatalogConfig config,
			PlayerActionDefinitionRegistry actionDefinitions,
			out PlayerCombatLoadoutCatalog catalog,
			out IReadOnlyList<string> errors
		)
		{
			var errorList = new List<string>();

			if (!ValidateCatalogRoot(config, actionDefinitions, errorList))
			{
				catalog = PlayerCombatLoadouts.CreateDefaultCatalog();
				errors = errorList;
				return false;
			}

			var loadouts = new Dictionary<string, PlayerCombatLoadout>(
				StringComparer.OrdinalIgnoreCase
			);
			for (int i = 0; i < config.Loadouts.Count; i++)
			{
				TryParseLoadout(config.Loadouts[i], i, actionDefinitions, loadouts, errorList);
			}

			if (
				!string.IsNullOrWhiteSpace(config.DefaultLoadoutId)
				&& !loadouts.ContainsKey(config.DefaultLoadoutId)
			)
			{
				errorList.Add(
					$"DefaultLoadoutId '{config.DefaultLoadoutId}' was not found in Loadouts."
				);
			}

			if (errorList.Count > 0)
			{
				catalog = PlayerCombatLoadouts.CreateDefaultCatalog();
				errors = errorList;
				return false;
			}

			catalog = new PlayerCombatLoadoutCatalog(config.DefaultLoadoutId, loadouts);
			errors = Array.Empty<string>();
			return true;
		}

		private static bool ValidateCatalogRoot(
			PlayerCombatLoadoutCatalogConfig config,
			PlayerActionDefinitionRegistry actionDefinitions,
			List<string> errors
		)
		{
			if (config == null)
			{
				errors.Add("Combat loadout catalog config is required.");
				return false;
			}

			if (actionDefinitions == null)
			{
				errors.Add("Action definition registry is required.");
				return false;
			}

			if (string.IsNullOrWhiteSpace(config.DefaultLoadoutId))
				errors.Add("DefaultLoadoutId is required.");

			if (config.Loadouts == null || config.Loadouts.Count == 0)
				errors.Add("Combat loadout catalog must contain at least one loadout.");

			return errors.Count == 0;
		}

		private static void TryParseLoadout(
			PlayerCombatLoadoutConfig loadoutConfig,
			int index,
			PlayerActionDefinitionRegistry actionDefinitions,
			IDictionary<string, PlayerCombatLoadout> loadouts,
			List<string> errors
		)
		{
			var location = $"Loadout entry #{index + 1}";
			var startErrorCount = errors.Count;

			if (loadoutConfig == null)
			{
				errors.Add($"{location} is null.");
				return;
			}

			if (string.IsNullOrWhiteSpace(loadoutConfig.Id))
			{
				errors.Add($"{location} Id is required.");
				return;
			}

			if (loadouts.ContainsKey(loadoutConfig.Id))
			{
				errors.Add($"{location} has duplicate loadout id '{loadoutConfig.Id}'.");
				return;
			}

			if (loadoutConfig.PrimarySlot == null)
				errors.Add($"{location} PrimarySlot is required.");

			if (loadoutConfig.SecondarySlot == null)
				errors.Add($"{location} SecondarySlot is required.");

			if (loadoutConfig.ActionSet == null)
				errors.Add($"{location} ActionSet is required.");

			if (errors.Count > startErrorCount)
				return;

			var loadout = new PlayerCombatLoadout();
			ApplySlotProfile(
				loadoutConfig.PrimarySlot,
				loadout.PrimarySlot,
				$"{location} PrimarySlot",
				errors
			);
			ApplySlotProfile(
				loadoutConfig.SecondarySlot,
				loadout.SecondarySlot,
				$"{location} SecondarySlot",
				errors
			);
			ApplyActionSet(
				loadoutConfig.ActionSet,
				loadout.ActionSet,
				location,
				actionDefinitions,
				errors
			);

			if (errors.Count > startErrorCount)
				return;

			loadouts.Add(loadoutConfig.Id, loadout);
		}

		private static void ApplySlotProfile(
			PlayerCombatSlotProfileConfig slotConfig,
			PlayerCombatSlotProfile slot,
			string location,
			List<string> errors
		)
		{
			if (
				TryParseRequiredEnum(
					slotConfig.SlotKind,
					location + ".SlotKind",
					out PlayerCombatSlotKind slotKind,
					errors
				)
			)
			{
				slot.SlotKind = slotKind;
			}

			if (
				TryParseRequiredEnum(
					slotConfig.ModifierPostureEffect,
					location + ".ModifierPostureEffect",
					out PlayerModifierPostureEffect postureEffect,
					errors
				)
			)
			{
				slot.ModifierPostureEffect = postureEffect;
			}
		}

		private static void ApplyActionSet(
			PlayerActionSetConfig actionSetConfig,
			PlayerActionSet actionSet,
			string location,
			PlayerActionDefinitionRegistry actionDefinitions,
			List<string> errors
		)
		{
			var startErrorCount = errors.Count;

			if (actionSetConfig.BaseBank == null)
				errors.Add($"{location} ActionSet.BaseBank is required.");

			if (actionSetConfig.PrimaryModifierBank == null)
				errors.Add($"{location} ActionSet.PrimaryModifierBank is required.");

			if (actionSetConfig.SecondaryModifierBank == null)
				errors.Add($"{location} ActionSet.SecondaryModifierBank is required.");

			if (errors.Count > startErrorCount)
				return;

			ApplyBank(
				actionSetConfig.BaseBank,
				actionSet.BaseBank,
				location + " ActionSet.BaseBank",
				actionDefinitions,
				errors
			);
			ApplyBank(
				actionSetConfig.PrimaryModifierBank,
				actionSet.PrimaryModifierBank,
				location + " ActionSet.PrimaryModifierBank",
				actionDefinitions,
				errors
			);
			ApplyBank(
				actionSetConfig.SecondaryModifierBank,
				actionSet.SecondaryModifierBank,
				location + " ActionSet.SecondaryModifierBank",
				actionDefinitions,
				errors
			);
		}

		private static void ApplyBank(
			PlayerActionBankConfig bankConfig,
			PlayerActionBank bank,
			string location,
			PlayerActionDefinitionRegistry actionDefinitions,
			List<string> errors
		)
		{
			bank.PrimaryFaceActionId = ParseMappedActionId(
				bankConfig.PrimaryFaceActionId,
				location + ".PrimaryFaceActionId",
				actionDefinitions,
				errors
			);
			bank.SecondaryFaceActionId = ParseMappedActionId(
				bankConfig.SecondaryFaceActionId,
				location + ".SecondaryFaceActionId",
				actionDefinitions,
				errors
			);
			bank.TertiaryFaceActionId = ParseMappedActionId(
				bankConfig.TertiaryFaceActionId,
				location + ".TertiaryFaceActionId",
				actionDefinitions,
				errors
			);
			bank.RightActionId = ParseMappedActionId(
				bankConfig.RightActionId,
				location + ".RightActionId",
				actionDefinitions,
				errors
			);
		}

		private static PlayerActionId ParseMappedActionId(
			string rawValue,
			string label,
			PlayerActionDefinitionRegistry actionDefinitions,
			List<string> errors
		)
		{
			if (!TryParseRequiredEnum(rawValue, label, out PlayerActionId id, errors))
				return PlayerActionId.None;

			if (id != PlayerActionId.None && !actionDefinitions.TryGet(id, out _))
				errors.Add(label + $" references missing action definition '{id}'.");

			return id;
		}

		private static bool TryParseRequiredEnum<TEnum>(
			string rawValue,
			string label,
			out TEnum value,
			List<string> errors
		)
			where TEnum : struct
		{
			if (string.IsNullOrWhiteSpace(rawValue))
			{
				errors.Add(label + " is required.");
				value = default;
				return false;
			}

			if (!Enum.TryParse(rawValue, true, out value))
			{
				errors.Add(label + " value '" + rawValue + "' is not recognized.");
				return false;
			}

			return true;
		}

		private static PlayerCombatLoadoutConfig CreateLoadoutConfig(
			string id,
			PlayerCombatLoadout loadout
		)
		{
			return new PlayerCombatLoadoutConfig
			{
				Id = id,
				PrimarySlot = CreateSlotConfig(loadout.PrimarySlot),
				SecondarySlot = CreateSlotConfig(loadout.SecondarySlot),
				ActionSet = CreateActionSetConfig(loadout.ActionSet),
			};
		}

		private static PlayerCombatSlotProfileConfig CreateSlotConfig(PlayerCombatSlotProfile slot)
		{
			return new PlayerCombatSlotProfileConfig
			{
				SlotKind = slot.SlotKind.ToString(),
				ModifierPostureEffect = slot.ModifierPostureEffect.ToString(),
			};
		}

		private static PlayerActionSetConfig CreateActionSetConfig(PlayerActionSet actionSet)
		{
			return new PlayerActionSetConfig
			{
				BaseBank = CreateBankConfig(actionSet.BaseBank),
				PrimaryModifierBank = CreateBankConfig(actionSet.PrimaryModifierBank),
				SecondaryModifierBank = CreateBankConfig(actionSet.SecondaryModifierBank),
			};
		}

		private static PlayerActionBankConfig CreateBankConfig(PlayerActionBank bank)
		{
			return new PlayerActionBankConfig
			{
				PrimaryFaceActionId = bank.PrimaryFaceActionId.ToString(),
				SecondaryFaceActionId = bank.SecondaryFaceActionId.ToString(),
				TertiaryFaceActionId = bank.TertiaryFaceActionId.ToString(),
				RightActionId = bank.RightActionId.ToString(),
			};
		}
	}
}
