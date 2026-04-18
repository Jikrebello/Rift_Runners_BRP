using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Input;

namespace Assets.Scripts.Game.Characters.Core.Player.Intent
{
	public sealed class PlayerIntentResolver
	{
		private readonly PlayerIntentConfig _config;

		public PlayerIntentResolver(PlayerIntentConfig config)
		{
			_config = config;
		}

		public IReadOnlyList<IPlayerIntent> Resolve(in PlayerInputSnapshot input)
		{
			var intents = new List<IPlayerIntent>(16) { new MoveIntent(input.Move) };

			AddJumpIntents(input, intents);
			AddToggleIntents(input, intents);
			AddModifierIntents(input, intents);
			AddActionIntents(input, intents);

			return intents;
		}

		private static void AddActionIntents(
			in PlayerInputSnapshot input,
			List<IPlayerIntent> intents
		)
		{
			EmitSkillOrBaseFaceIntents(input, intents);
			EmitRightActionIntent(input, intents);
			EmitTraversalFaceIntents(input, intents);
		}

		private static void AddJumpIntents(
			in PlayerInputSnapshot input,
			List<IPlayerIntent> intents
		)
		{
			if (input.Jump.PressedThisFrame)
			{
				intents.Add(new JumpPressedIntent());
				intents.Add(new JumpHeldIntent(true));
			}

			if (input.Jump.ReleasedThisFrame)
			{
				intents.Add(new JumpHeldIntent(false));
				intents.Add(new JumpReleasedIntent());
			}
		}

		private static void AddToggleIntents(
			in PlayerInputSnapshot input,
			List<IPlayerIntent> intents
		)
		{
			if (input.ToggleCrouch.PressedThisFrame)
				intents.Add(new ToggleCrouchIntent());

			if (input.ToggleSprint.PressedThisFrame)
				intents.Add(new ToggleSprintIntent());

			if (input.ToggleWeaponStance.PressedThisFrame)
				intents.Add(new ToggleWeaponStanceIntent());
		}

		private static void EmitBaseFaceIntents(
			in PlayerInputSnapshot input,
			List<IPlayerIntent> intents
		)
		{
			if (input.Primary.PressedThisFrame)
				intents.Add(new PrimaryPressedIntent());

			if (input.Secondary.PressedThisFrame)
				intents.Add(new SecondaryPressedIntent());

			if (input.Tertiary.PressedThisFrame)
				intents.Add(new CombatTertiaryPressedIntent());
		}

		private static void EmitRightActionIntent(
			in PlayerInputSnapshot input,
			List<IPlayerIntent> intents
		)
		{
			if (input.ContextGrabOrFire.PressedThisFrame)
				intents.Add(new RightActionIntent());
		}

		private static void EmitSkillOrBaseFaceIntents(
			in PlayerInputSnapshot input,
			List<IPlayerIntent> intents
		)
		{
			EmitBaseFaceIntents(input, intents);
		}

		private static void EmitTraversalFaceIntents(
			in PlayerInputSnapshot input,
			List<IPlayerIntent> intents
		)
		{
			// This is the “circle pressed” signal traversal needs for slide/drop/etc.
			if (input.Tertiary.PressedThisFrame)
				intents.Add(new TertiaryPressedIntent());
		}

		private void AddModifierIntents(in PlayerInputSnapshot input, List<IPlayerIntent> intents)
		{
			if (!_config.EmitModifierHeldTransitions)
				return;

			// PrimarySkillModifier = R1 (skill modifier)
			if (
				input.PrimarySkillModifier.PressedThisFrame
				|| input.PrimarySkillModifier.ReleasedThisFrame
			)
				intents.Add(new PrimaryModifierHeldIntent(input.PrimarySkillModifier.Held));

			// SecondarySkillModifier = L1 (aim/block modifier)
			if (
				input.SecondarySkillModifier.PressedThisFrame
				|| input.SecondarySkillModifier.ReleasedThisFrame
			)
				intents.Add(new SecondaryModifierHeldIntent(input.SecondarySkillModifier.Held));
		}
	}
}
