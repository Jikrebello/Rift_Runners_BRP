using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	public sealed class ActionDefinitionRegistryIntegrationTests
	{
		[Test]
		public void ExplicitlyLoadedRegistry_LightAttackRecoveryHeavyAttackRequest_CancelsImmediatelyIntoHeavyAttack()
		{
			var registry = PlayerActionDefinitionCatalogLoader.LoadExisting(
				GetCommittedCatalogPath()
			);
			var system = new ActionTestDriver(registry);
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);

			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Recovery));

			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new HeavyAttackIntent() },
				dt: 0f
			);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.HeavyAttack)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
			Assert.That(
				outputs.Animation.Triggers.Any(x => x.Param == AnimTrigger.HeavyAttack),
				Is.True
			);
		}

		[Test]
		public void ExplicitlyLoadedRegistry_RepeatedLightAttackStillPromotesIntoLightAttack2()
		{
			var registry = PlayerActionDefinitionCatalogLoader.LoadExisting(
				GetCommittedCatalogPath()
			);
			var system = new ActionTestDriver(registry);
			var model = new PlayerModel();
			var outputs = new PlayerOutputs();

			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);

			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.11f);
			outputs.Clear();
			system.Step(
				model,
				outputs,
				new List<IPlayerIntent> { new LightAttackIntent() },
				dt: 0f
			);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.09f);
			outputs.Clear();
			system.Step(model, outputs, new List<IPlayerIntent>(), dt: 0.23f);

			Assert.That(
				model.ActionRuntime.CurrentActionId,
				Is.EqualTo(PlayerActionId.LightAttack2)
			);
			Assert.That(model.ActionRuntime.CurrentPhase, Is.EqualTo(PlayerActionPhase.Startup));
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
	}
}
