using System.Collections.Generic;
using System.IO;
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
	public sealed class CombatLoadoutCatalogIntegrationTests
	{
		[Test]
		public void ExplicitlyLoadedDefaultLoadout_PreservesBaseAndModifierResolution()
		{
			var definitions = PlayerActionDefinitionCatalogLoader.LoadExisting(
				GetCommittedActionCatalogPath()
			);
			var loadoutCatalog = PlayerCombatLoadoutCatalogLoader.LoadExisting(
				definitions,
				GetCommittedCombatLoadoutCatalogPath()
			);
			var system = new ActionTestDriver(definitions);
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			PlayerCombatLoadouts.CopyInto(loadoutCatalog.GetDefaultLoadout(), model.CombatLoadout);

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);
			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack)
			);

			model.ActionRuntime.ClearCurrent();
			model.ActionRuntime.ClearBuffered();
			outputs.Clear();
			model.PrimaryMode = PrimaryModifierMode.Active;
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);
			Assert.That(model.ActionRuntime.CurrentActionId, Is.EqualTo(PlayerActionId.Skill3));

			model.ActionRuntime.ClearCurrent();
			model.ActionRuntime.ClearBuffered();
			outputs.Clear();
			model.PrimaryMode = PrimaryModifierMode.None;
			model.SecondaryMode = SecondaryModifierMode.Active;
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);
			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.FundamentalRangedPrimary)
			);
		}

		[Test]
		public void ExplicitlyLoadedDefaultLoadout_PreservesContextInteractSpecialCase()
		{
			var definitions = PlayerActionDefinitionCatalogLoader.LoadExisting(
				GetCommittedActionCatalogPath()
			);
			var loadoutCatalog = PlayerCombatLoadoutCatalogLoader.LoadExisting(
				definitions,
				GetCommittedCombatLoadoutCatalogPath()
			);
			var system = new ActionTestDriver(definitions);
			var model = new PlayerModel { TraversalMode = PlayerTraversalMode.Grounded };
			var outputs = new PlayerOutputs();

			PlayerCombatLoadouts.CopyInto(loadoutCatalog.GetDefaultLoadout(), model.CombatLoadout);
			model.CombatLoadout.ActionSet.BaseBank.ContextInteractId = PlayerActionId.None;

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new ContextInteractIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.ContextInteract)
			);
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
	}
}
