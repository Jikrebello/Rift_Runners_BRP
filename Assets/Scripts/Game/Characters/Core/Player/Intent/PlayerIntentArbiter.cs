using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Traversal;

namespace Assets.Scripts.Game.Characters.Core.Player.Intent
{
	public static class PlayerIntentArbiter
	{
		public static IReadOnlyList<IPlayerIntent> Arbitrate(
			PlayerModel model,
			in PlayerWorldSnapshot _,
			IReadOnlyList<IPlayerIntent> rawIntents
		)
		{
			ScanFrame(
				rawIntents,
				out bool hasCombatTertiaryPressed,
				out bool hasTertiaryPressed
			);

			bool circleIsTraversal = ComputeCircleIsTraversal(model, hasTertiaryPressed);

			var filtered = Filter(rawIntents, hasCombatTertiaryPressed, circleIsTraversal);

			return OrderByPriority(filtered);
		}

		private static void AddAll<T>(List<IPlayerIntent> src, List<IPlayerIntent> dst)
			where T : class, IPlayerIntent
		{
			for (int i = 0; i < src.Count; i++)
			{
				if (src[i] is T)
					dst.Add(src[i]);
			}
		}

		private static void AppendUnknown(List<IPlayerIntent> intents, List<IPlayerIntent> ordered)
		{
			for (int i = 0; i < intents.Count; i++)
			{
				var x = intents[i];
				if (!ContainsReference(ordered, x))
					ordered.Add(x);
			}
		}

		private static bool ComputeCircleIsTraversal(PlayerModel model, bool hasTertiaryPressed)
		{
			if (!hasTertiaryPressed)
				return false;

			return model.TraversalMode == PlayerTraversalMode.Airborne
				|| model.GroundedSubMode == PlayerGroundedSubMode.Sprinting;
		}

		private static bool ContainsReference(List<IPlayerIntent> list, IPlayerIntent item)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (ReferenceEquals(list[i], item))
					return true;
			}
			return false;
		}

		private static List<IPlayerIntent> Filter(
			IReadOnlyList<IPlayerIntent> rawIntents,
			bool hasCombatTertiaryPressed,
			bool circleIsTraversal
		)
		{
			var result = new List<IPlayerIntent>(rawIntents.Count);

			for (int i = 0; i < rawIntents.Count; i++)
			{
				var intent = rawIntents[i];

				if (ShouldDrop(intent, hasCombatTertiaryPressed, circleIsTraversal))
					continue;

				result.Add(intent);
			}

			return result;
		}

		private static IReadOnlyList<IPlayerIntent> OrderByPriority(List<IPlayerIntent> intents)
		{
			var ordered = new List<IPlayerIntent>(intents.Count);

			// Bucket 1: combat mode (stance/modifier toggles)
			AddAll<ToggleWeaponStanceIntent>(intents, ordered);
			AddAll<PrimaryModifierHeldIntent>(intents, ordered);
			AddAll<SecondaryModifierHeldIntent>(intents, ordered);

			// Bucket 2: traversal control (jump + tertiary traversal)
			AddAll<JumpPressedIntent>(intents, ordered);
			AddAll<JumpReleasedIntent>(intents, ordered);
			AddAll<TertiaryPressedIntent>(intents, ordered);
			AddAll<ToggleSprintIntent>(intents, ordered);
			AddAll<ToggleCrouchIntent>(intents, ordered);

			// Bucket 3: actions
			AddAll<PrimaryPressedIntent>(intents, ordered);
			AddAll<SecondaryPressedIntent>(intents, ordered);
			AddAll<CombatTertiaryPressedIntent>(intents, ordered);
			AddAll<LightAttackIntent>(intents, ordered);
			AddAll<HeavyAttackIntent>(intents, ordered);
			AddAll<RightActionIntent>(intents, ordered);
			AddAll<ContextInteractIntent>(intents, ordered);

			// Bucket 4: movement (already kept, but include for completeness)
			AddAll<MoveIntent>(intents, ordered);

			AppendUnknown(intents, ordered);

			return ordered;
		}

		private static void ScanFrame(
			IReadOnlyList<IPlayerIntent> rawIntents,
			out bool hasCombatTertiaryPressed,
			out bool hasTertiaryPressed
		)
		{
			hasCombatTertiaryPressed = false;
			hasTertiaryPressed = false;

			for (int i = 0; i < rawIntents.Count; i++)
			{
				var intent = rawIntents[i];

				if (intent is CombatTertiaryPressedIntent)
					hasCombatTertiaryPressed = true;

				if (intent is TertiaryPressedIntent)
					hasTertiaryPressed = true;
			}
		}

		private static bool ShouldDrop(
			IPlayerIntent intent,
			bool hasCombatTertiaryPressed,
			bool circleIsTraversal
		)
		{
			// Always keep movement
			if (intent is MoveIntent)
				return false;

			if (
				circleIsTraversal
				&& (intent is CombatTertiaryPressedIntent || intent is ContextInteractIntent)
			)
				return true;

			if (
				!circleIsTraversal
				&& hasCombatTertiaryPressed
				&& intent is TertiaryPressedIntent
			)
				return true;

			return false;
		}
	}
}
