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
				new List<IPlayerIntent> { new PrimaryPressedIntent() },
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
				new List<IPlayerIntent> { new CombatTertiaryPressedIntent() },
				dt: 0f
			);
			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.SwordSkillTertiary)
			);

			model.ActionRuntime.ClearCurrent();
			model.ActionRuntime.ClearBuffered();
			outputs.Clear();
			model.PrimaryMode = PrimaryModifierMode.None;
			model.SecondaryMode = SecondaryModifierMode.Active;
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new CombatTertiaryPressedIntent() },
				dt: 0f
			);
			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.ShieldSkillTertiary)
			);
			Assert.That(
				outputs.Animation.Triggers,
				Has.Some.Matches<TriggerCmd>(x =>
					x.Param == AnimTrigger.ShieldSkillTertiary
				)
			);
		}

		[Test]
		public void ExplicitlyLoadedDefaultLoadout_SecondaryModifierRightAction_RemainsShieldFundamental()
		{
			var definitions = PlayerActionDefinitionCatalogLoader.LoadExisting(
				GetCommittedActionCatalogPath()
			);
			var loadoutCatalog = PlayerCombatLoadoutCatalogLoader.LoadExisting(
				definitions,
				GetCommittedCombatLoadoutCatalogPath()
			);
			var system = new ActionTestDriver(definitions);
			var model = new PlayerModel
			{
				TraversalMode = PlayerTraversalMode.Grounded,
				SecondaryMode = SecondaryModifierMode.Active,
			};
			var outputs = new PlayerOutputs();

			PlayerCombatLoadouts.CopyInto(loadoutCatalog.GetDefaultLoadout(), model.CombatLoadout);

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new RightActionIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.FundamentalBlockPrimary)
			);
			Assert.That(
				outputs.Animation.Triggers,
				Has.Some.Matches<TriggerCmd>(x =>
					x.Param == AnimTrigger.FundamentalBlockPrimary
				)
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
