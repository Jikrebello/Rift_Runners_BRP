using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions.Config;
using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;
using Assets.Scripts.Game.Characters.Core.Player.Intent;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;
using NUnit.Framework;

namespace Assets.Tests.EditMode
{
	public sealed class ActionDefinitionRegistryTests
	{
		[Test]
		public void CatalogLoader_LoadsCommittedActionCatalog_ProducesExpectedDefinitions()
		{
			var path = GetCommittedCatalogPath();

			var registry = PlayerActionDefinitionCatalogLoader.LoadExisting(path);

			var lightAttack = registry.Get(PlayerActionId.LightAttack);
			Assert.That(lightAttack.Timing.StartupSeconds, Is.EqualTo(0.10f));
			Assert.That(
				lightAttack.CancelPolicy.Window,
				Is.EqualTo(PlayerActionBufferWindow.RecoveryOnly)
			);
			Assert.That(
				lightAttack.CancelPolicy.AllowsCancelTo(
					PlayerActionId.HeavyAttack,
					PlayerActionPhase.Recovery
				),
				Is.True
			);

			var heavyAttack = registry.Get(PlayerActionId.HeavyAttack);
			Assert.That(
				heavyAttack.Execution.BufferWindow,
				Is.EqualTo(PlayerActionBufferWindow.RecoveryOnly)
			);

			var skill1 = registry.Get(PlayerActionId.Skill1);
			Assert.That(skill1.Execution.StaminaCost, Is.EqualTo(20f));

			var contextInteract = registry.Get(PlayerActionId.ContextInteract);
			Assert.That(contextInteract.Source, Is.EqualTo(PlayerActionSource.Context));
			Assert.That(contextInteract.Category, Is.EqualTo(PlayerActionCategory.Interact));

			var blockPrimary = registry.Get(PlayerActionId.FundamentalBlockPrimary);
			Assert.That(blockPrimary.Source, Is.EqualTo(PlayerActionSource.Fundamental));
			Assert.That(blockPrimary.Category, Is.EqualTo(PlayerActionCategory.Attack));
			Assert.That(
				blockPrimary.AnimationTrigger,
				Is.EqualTo(AnimTrigger.FundamentalBlockPrimary)
			);
		}

		[Test]
		public void SaveDefaultCatalog_CreatesCatalogThatLoadsSuccessfully()
		{
			var path = GetTempCatalogPath();

			PlayerActionDefinitionCatalogLoader.SaveDefaultCatalog(path);

			Assert.That(File.Exists(path), Is.True);

			var registry = PlayerActionDefinitionCatalogLoader.LoadExisting(path);

			Assert.That(
				registry.Get(PlayerActionId.LightAttack).Id,
				Is.EqualTo(PlayerActionId.LightAttack)
			);
			Assert.That(
				registry.Get(PlayerActionId.HeavyAttack).Id,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);
		}

		[Test]
		public void CatalogBuilder_RejectsDuplicateActionIds()
		{
			var config = PlayerActionDefinitionCatalogLoader.CreateDefaultCatalog();
			config.Actions[1].Id = config.Actions[0].Id;

			var errors = GetBuildErrors(config);

			Assert.That(errors, Has.Some.Contains("duplicate action id"));
		}

		[Test]
		public void CatalogBuilder_RejectsUnknownEnumNames()
		{
			var badActionId = PlayerActionDefinitionCatalogLoader.CreateDefaultCatalog();
			badActionId.Actions[0].Id = "MissingAction";
			Assert.That(
				GetBuildErrors(badActionId),
				Has.Some.Contains("Id value 'MissingAction' is not recognized.")
			);

			var badBufferWindow = PlayerActionDefinitionCatalogLoader.CreateDefaultCatalog();
			badBufferWindow.Actions[0].Execution.BufferWindow = "LateRecovery";
			Assert.That(
				GetBuildErrors(badBufferWindow),
				Has.Some.Contains("Execution.BufferWindow value 'LateRecovery' is not recognized.")
			);

			var badAnimationTrigger = PlayerActionDefinitionCatalogLoader.CreateDefaultCatalog();
			badAnimationTrigger.Actions[0].AnimationTrigger = "SpinSlash";
			Assert.That(
				GetBuildErrors(badAnimationTrigger),
				Has.Some.Contains("AnimationTrigger value 'SpinSlash' is not recognized.")
			);
		}

		[Test]
		public void CatalogBuilder_RejectsUnknownCancelTargets()
		{
			var config = PlayerActionDefinitionCatalogLoader.CreateDefaultCatalog();
			config.Actions[0].CancelPolicy = new PlayerActionCancelPolicyConfig
			{
				Window = "RecoveryOnly",
				AllowedTargetIds = new List<string> { "MissingAction" },
			};

			var errors = GetBuildErrors(config);

			Assert.That(
				errors,
				Has.Some.Contains(
					"CancelPolicy.AllowedTargetIds[0] value 'MissingAction' is not recognized."
				)
			);
		}

		[Test]
		public void CatalogBuilder_RejectsInvalidTimings()
		{
			var negativeTiming = PlayerActionDefinitionCatalogLoader.CreateDefaultCatalog();
			negativeTiming.Actions[0].Timing.StartupSeconds = -0.01f;
			Assert.That(
				GetBuildErrors(negativeTiming),
				Has.Some.Contains("StartupSeconds must be >= 0.")
			);

			var zeroTotalTiming = PlayerActionDefinitionCatalogLoader.CreateDefaultCatalog();
			zeroTotalTiming.Actions[0].Timing.StartupSeconds = 0f;
			zeroTotalTiming.Actions[0].Timing.ActiveSeconds = 0f;
			zeroTotalTiming.Actions[0].Timing.RecoverySeconds = 0f;
			Assert.That(
				GetBuildErrors(zeroTotalTiming),
				Has.Some.Contains("total action duration must be > 0.")
			);
		}

		[Test]
		public void CatalogBuilder_RejectsExecutionWindowMismatch()
		{
			var nonBufferableWindow = PlayerActionDefinitionCatalogLoader.CreateDefaultCatalog();
			nonBufferableWindow.Actions[0].Execution.CanBuffer = false;
			Assert.That(
				GetBuildErrors(nonBufferableWindow),
				Has.Some.Contains("cannot specify a buffer window when CanBuffer is false")
			);

			var missingBufferWindow = PlayerActionDefinitionCatalogLoader.CreateDefaultCatalog();
			missingBufferWindow.Actions[0].Execution.BufferWindow =
				PlayerActionBufferWindow.None.ToString();
			Assert.That(
				GetBuildErrors(missingBufferWindow),
				Has.Some.Contains("must specify a non-None buffer window when CanBuffer is true")
			);
		}

		[Test]
		public void DefaultActionRuntime_DoesNotCreateCatalogFileAsSideEffect()
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
					"player_action_definitions.json"
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

		private static IReadOnlyList<string> GetBuildErrors(
			PlayerActionDefinitionCatalogConfig config
		)
		{
			var success = PlayerActionDefinitionCatalogBuilder.TryBuild(
				config,
				out _,
				out var errors
			);

			Assert.That(success, Is.False);
			return errors;
		}

		private static string GetCommittedCatalogPath([CallerFilePath] string sourcePath = "")
		{
			var sourceDirectory = Path.GetDirectoryName(sourcePath)!;
			return Path.GetFullPath(
				Path.Combine(
					sourceDirectory,
					"..",
					"..",
					"..",
					"Configs",
					"Player",
					"player_action_definitions.json"
				)
			);
		}

		private static string GetTempCatalogPath()
		{
			return Path.Combine(
				Path.GetTempPath(),
				Guid.NewGuid().ToString("N"),
				"player_action_definitions.json"
			);
		}
	}
}
