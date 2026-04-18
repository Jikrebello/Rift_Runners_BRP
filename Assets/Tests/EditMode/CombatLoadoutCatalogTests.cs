using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions.Config;
using Assets.Scripts.Game.Characters.Core.Player.Action.Loadout;
using Assets.Scripts.Game.Characters.Core.Player.Action.Loadout.Config;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class CombatLoadoutCatalogTests
	{
		[Test]
		public void CatalogLoader_LoadsCommittedDefaultCombatLoadout()
		{
			var definitions = LoadActionDefinitions();
			var path = GetCommittedCombatLoadoutCatalogPath();

			var catalog = PlayerCombatLoadoutCatalogLoader.LoadExisting(definitions, path);
			var loadout = catalog.GetDefaultLoadout();

			Assert.That(
				catalog.DefaultLoadoutId,
				Is.EqualTo(PlayerCombatLoadouts.DefaultLoadoutId)
			);
			Assert.That(loadout.PrimarySlot.SlotKind, Is.EqualTo(PlayerCombatSlotKind.Sword));
			Assert.That(loadout.SecondarySlot.SlotKind, Is.EqualTo(PlayerCombatSlotKind.Shield));
			Assert.That(
				loadout.SecondarySlot.ModifierPostureEffect,
				Is.EqualTo(PlayerModifierPostureEffect.Block)
			);
			Assert.That(
				loadout.ActionSet.BaseBank.PrimaryFaceActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
			Assert.That(
				loadout.ActionSet.BaseBank.TertiaryFaceActionId,
				Is.EqualTo(PlayerActionId.ContextInteract)
			);
			Assert.That(
				loadout.ActionSet.SecondaryModifierBank.RightActionId,
				Is.EqualTo(PlayerActionId.FundamentalBlockPrimary)
			);
			Assert.That(
				loadout.ActionSet.PrimaryModifierBank.PrimaryFaceActionId,
				Is.EqualTo(PlayerActionId.SwordAdvanceSlash)
			);
			Assert.That(
				loadout.ActionSet.SecondaryModifierBank.TertiaryFaceActionId,
				Is.EqualTo(PlayerActionId.ShieldSkillTertiary)
			);
		}

		[Test]
		public void SaveDefaultCatalog_CreatesCatalogThatLoadsSuccessfully()
		{
			var definitions = LoadActionDefinitions();
			var path = GetTempCatalogPath();

			PlayerCombatLoadoutCatalogLoader.SaveDefaultCatalog(path);

			Assert.That(File.Exists(path), Is.True);

			var catalog = PlayerCombatLoadoutCatalogLoader.LoadExisting(definitions, path);
			var loadout = catalog.GetDefaultLoadout();

			Assert.That(
				catalog.LoadoutIds.Single(),
				Is.EqualTo(PlayerCombatLoadouts.DefaultLoadoutId)
			);
			Assert.That(loadout.PrimarySlot.SlotKind, Is.EqualTo(PlayerCombatSlotKind.Sword));
		}

		[Test]
		public void CatalogBuilder_RejectsDuplicateLoadoutIds()
		{
			var config = PlayerCombatLoadoutCatalogLoader.CreateDefaultCatalog();
			config.Loadouts.Add(CloneLoadoutConfig(config.Loadouts[0]));

			var errors = GetBuildErrors(config);

			Assert.That(errors, Has.Some.Contains("duplicate loadout id"));
		}

		[Test]
		public void CatalogBuilder_RejectsUnknownSlotKind()
		{
			var config = PlayerCombatLoadoutCatalogLoader.CreateDefaultCatalog();
			config.Loadouts[0].PrimarySlot.SlotKind = "Hammer";

			var errors = GetBuildErrors(config);

			Assert.That(
				errors,
				Has.Some.Contains("PrimarySlot.SlotKind value 'Hammer' is not recognized.")
			);
		}

		[Test]
		public void CatalogBuilder_RejectsUnknownModifierPostureEffect()
		{
			var config = PlayerCombatLoadoutCatalogLoader.CreateDefaultCatalog();
			config.Loadouts[0].SecondarySlot.ModifierPostureEffect = "GuardAim";

			var errors = GetBuildErrors(config);

			Assert.That(
				errors,
				Has.Some.Contains(
					"SecondarySlot.ModifierPostureEffect value 'GuardAim' is not recognized."
				)
			);
		}

		[Test]
		public void CatalogBuilder_RejectsMissingRequiredBanks()
		{
			var config = PlayerCombatLoadoutCatalogLoader.CreateDefaultCatalog();
			config.Loadouts[0].ActionSet.SecondaryModifierBank = null;

			var errors = GetBuildErrors(config);

			Assert.That(errors, Has.Some.Contains("ActionSet.SecondaryModifierBank is required."));
		}

		[Test]
		public void CatalogBuilder_RejectsUnknownActionIdValues()
		{
			var config = PlayerCombatLoadoutCatalogLoader.CreateDefaultCatalog();
			config.Loadouts[0].ActionSet.BaseBank.PrimaryFaceActionId = "SpiralStrike";

			var errors = GetBuildErrors(config);

			Assert.That(
				errors,
				Has.Some.Contains(
					"BaseBank.PrimaryFaceActionId value 'SpiralStrike' is not recognized."
				)
			);
		}

		[Test]
		public void CatalogBuilder_RejectsMissingActionDefinitionReferences()
		{
			var config = PlayerCombatLoadoutCatalogLoader.CreateDefaultCatalog();
			var limitedDefinitions = CreateDefinitionRegistryWithout(
				PlayerActionId.ShieldSkillTertiary
			);

			var success = PlayerCombatLoadoutCatalogBuilder.TryBuild(
				config,
				limitedDefinitions,
				out _,
				out var errors
			);

			Assert.That(success, Is.False);
			Assert.That(
				errors,
				Has.Some.Contains("references missing action definition 'ShieldSkillTertiary'")
			);
		}

		[Test]
		public void DefaultActionRuntime_DoesNotCreateLoadoutCatalogFileAsSideEffect()
		{
			var originalDirectory = Directory.GetCurrentDirectory();
			var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDirectory);
			Directory.SetCurrentDirectory(tempDirectory);

			try
			{
				var driver = new ActionTestDriver();
				var model = new PlayerModel();
				var outputs = new PlayerOutputs();

				driver.Step(
					model,
					outputs,
					new List<IPlayerIntent> { new LightAttackIntent() },
					dt: 0f
				);

				var expectedCatalogPath = Path.Combine(
					tempDirectory,
					"Configs",
					"Player",
					"player_combat_loadouts.json"
				);

				Assert.That(
					model.ActionRuntime.CurrentActionId,
					Is.EqualTo(PlayerActionId.LightAttack)
				);
				Assert.That(File.Exists(expectedCatalogPath), Is.False);
			}
			finally
			{
				Directory.SetCurrentDirectory(originalDirectory);
			}
		}

		private static PlayerActionDefinitionRegistry LoadActionDefinitions()
		{
			return PlayerActionDefinitionCatalogLoader.LoadExisting(
				GetCommittedActionCatalogPath()
			);
		}

		private static IReadOnlyList<string> GetBuildErrors(PlayerCombatLoadoutCatalogConfig config)
		{
			var success = PlayerCombatLoadoutCatalogBuilder.TryBuild(
				config,
				LoadActionDefinitions(),
				out _,
				out var errors
			);

			Assert.That(success, Is.False);
			return errors;
		}

		private static PlayerActionDefinitionRegistry CreateDefinitionRegistryWithout(
			PlayerActionId removedId
		)
		{
			var defaultDefinitions = PlayerActionDefinitions
				.CreateDefaultRegistry()
				.Definitions.Where(x => x.Id != removedId);

			return new PlayerActionDefinitionRegistry(defaultDefinitions);
		}

		private static PlayerCombatLoadoutConfig CloneLoadoutConfig(
			PlayerCombatLoadoutConfig source
		)
		{
			return new PlayerCombatLoadoutConfig
			{
				Id = source.Id,
				PrimarySlot = new PlayerCombatSlotProfileConfig
				{
					SlotKind = source.PrimarySlot.SlotKind,
					ModifierPostureEffect = source.PrimarySlot.ModifierPostureEffect,
				},
				SecondarySlot = new PlayerCombatSlotProfileConfig
				{
					SlotKind = source.SecondarySlot.SlotKind,
					ModifierPostureEffect = source.SecondarySlot.ModifierPostureEffect,
				},
				ActionSet = new PlayerActionSetConfig
				{
					BaseBank = CloneBankConfig(source.ActionSet.BaseBank),
					PrimaryModifierBank = CloneBankConfig(source.ActionSet.PrimaryModifierBank),
					SecondaryModifierBank = CloneBankConfig(source.ActionSet.SecondaryModifierBank),
				},
			};
		}

		private static PlayerActionBankConfig CloneBankConfig(PlayerActionBankConfig source)
		{
			return new PlayerActionBankConfig
			{
				PrimaryFaceActionId = source.PrimaryFaceActionId,
				SecondaryFaceActionId = source.SecondaryFaceActionId,
				TertiaryFaceActionId = source.TertiaryFaceActionId,
				RightActionId = source.RightActionId,
			};
		}

		private static string GetCommittedActionCatalogPath([CallerFilePath] string sourcePath = "")
		{
			return GetPathFromRepoRoot(
				sourcePath,
				"Configs",
				"Player",
				"player_action_definitions.json"
			);
		}

		private static string GetCommittedCombatLoadoutCatalogPath(
			[CallerFilePath] string sourcePath = ""
		)
		{
			return GetPathFromRepoRoot(
				sourcePath,
				"Configs",
				"Player",
				"player_combat_loadouts.json"
			);
		}

		private static string GetPathFromRepoRoot(
			string sourcePath,
			params string[] relativeSegments
		)
		{
			var sourceDirectory = Path.GetDirectoryName(sourcePath)!;
			var segments = new List<string> { sourceDirectory, "..", "..", ".." };
			segments.AddRange(relativeSegments);
			return Path.GetFullPath(Path.Combine(segments.ToArray()));
		}

		private static string GetTempCatalogPath()
		{
			return Path.Combine(
				Path.GetTempPath(),
				Guid.NewGuid().ToString("N"),
				"player_combat_loadouts.json"
			);
		}
	}
}
