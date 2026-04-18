#nullable disable
using System;
using System.Collections.Generic;
using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Action.Definitions.Config
{
	public static class PlayerActionDefinitionCatalogBuilder
	{
		public static PlayerActionDefinitionCatalogConfig CreateConfig(
			IReadOnlyList<PlayerActionDefinition> definitions
		)
		{
			if (definitions == null)
				throw new ArgumentNullException(nameof(definitions));

			var config = new PlayerActionDefinitionCatalogConfig();
			for (int i = 0; i < definitions.Count; i++)
			{
				config.Actions.Add(CreateActionConfig(definitions[i]));
			}

			return config;
		}

		public static bool TryBuild(
			PlayerActionDefinitionCatalogConfig config,
			out PlayerActionDefinitionRegistry registry,
			out IReadOnlyList<string> errors
		)
		{
			var errorList = new List<string>();

			if (!ValidateCatalogRoot(config, errorList))
			{
				registry = CreateEmptyRegistry();
				errors = errorList;
				return false;
			}

			var idsInCatalog = new HashSet<PlayerActionId>();
			var parsedEntries = ParseEntries(config.Actions, idsInCatalog, errorList);
			ValidateCancelTargets(parsedEntries, idsInCatalog, errorList);

			if (errorList.Count > 0)
			{
				registry = CreateEmptyRegistry();
				errors = errorList;
				return false;
			}

			registry = new PlayerActionDefinitionRegistry(BuildDefinitions(parsedEntries));
			errors = Array.Empty<string>();
			return true;
		}

		private static bool ValidateCatalogRoot(
			PlayerActionDefinitionCatalogConfig config,
			List<string> errors
		)
		{
			if (config == null)
			{
				errors.Add("Action definition catalog config is required.");
				return false;
			}

			if (config.Actions == null || config.Actions.Count == 0)
			{
				errors.Add("Action definition catalog must contain at least one action.");
				return false;
			}

			return true;
		}

		private static List<ParsedActionEntry> ParseEntries(
			List<PlayerActionDefinitionConfig> actions,
			HashSet<PlayerActionId> idsInCatalog,
			List<string> errors
		)
		{
			var parsedEntries = new List<ParsedActionEntry>();

			for (int i = 0; i < actions.Count; i++)
			{
				if (
					TryParseActionConfig(
						actions[i],
						i,
						idsInCatalog,
						errors,
						out ParsedActionEntry parsedEntry
					)
				)
				{
					parsedEntries.Add(parsedEntry);
				}
			}

			return parsedEntries;
		}

		private static void ValidateCancelTargets(
			List<ParsedActionEntry> parsedEntries,
			HashSet<PlayerActionId> idsInCatalog,
			List<string> errors
		)
		{
			for (int i = 0; i < parsedEntries.Count; i++)
			{
				var parsed = parsedEntries[i];
				for (
					int targetIndex = 0;
					targetIndex < parsed.Cancel.AllowedTargetIds.Length;
					targetIndex++
				)
				{
					var targetId = parsed.Cancel.AllowedTargetIds[targetIndex];
					if (!idsInCatalog.Contains(targetId))
					{
						errors.Add(
							$"Action '{parsed.Id}' references missing cancel target '{targetId}'."
						);
					}
				}
			}
		}

		private static IEnumerable<PlayerActionDefinition> BuildDefinitions(
			List<ParsedActionEntry> parsedEntries
		)
		{
			var definitions = new PlayerActionDefinition[parsedEntries.Count];

			for (int i = 0; i < parsedEntries.Count; i++)
			{
				var parsed = parsedEntries[i];
				definitions[i] = new PlayerActionDefinition(
					parsed.Id,
					parsed.Source,
					parsed.Category,
					parsed.AnimationTrigger,
					new PlayerActionTiming(
						parsed.Timing.StartupSeconds,
						parsed.Timing.ActiveSeconds,
						parsed.Timing.RecoverySeconds
					),
					new PlayerActionAvailability(
						parsed.Availability.RequiresGrounded,
						parsed.Availability.AllowWhileAirborne
					),
					new PlayerActionExecutionPolicy(
						parsed.Execution.CanBuffer,
						parsed.Execution.StaminaCost,
						parsed.Execution.BufferWindow
					),
					BuildMotorProfile(parsed.Motor),
					BuildCancelPolicy(parsed.Cancel)
				);
			}

			return definitions;
		}

		private static PlayerActionCancelPolicy BuildCancelPolicy(ParsedCancelPolicy cancel)
		{
			if (cancel.AllowedTargetIds.Length == 0)
				return PlayerActionCancelPolicy.None;

			return new PlayerActionCancelPolicy(cancel.Window, cancel.AllowedTargetIds);
		}

		private static PlayerActionMotorProfile BuildMotorProfile(ParsedMotor motor)
		{
			if (motor.Mode == PlayerActionMotorMode.None)
				return PlayerActionMotorProfile.None;

			return new PlayerActionMotorProfile(motor.Mode, motor.Phase, motor.MoveMultiplier);
		}

		private static PlayerActionDefinitionConfig CreateActionConfig(
			PlayerActionDefinition definition
		)
		{
			return new PlayerActionDefinitionConfig
			{
				Id = definition.Id.ToString(),
				Source = definition.Source.ToString(),
				Category = definition.Category.ToString(),
				AnimationTrigger = definition.AnimationTrigger.ToString(),
				Timing = new PlayerActionTimingConfig
				{
					StartupSeconds = definition.Timing.StartupSeconds,
					ActiveSeconds = definition.Timing.ActiveSeconds,
					RecoverySeconds = definition.Timing.RecoverySeconds,
				},
				Availability = new PlayerActionAvailabilityConfig
				{
					RequiresGrounded = definition.Availability.RequiresGrounded,
					AllowWhileAirborne = definition.Availability.AllowWhileAirborne,
				},
				Execution = new PlayerActionExecutionPolicyConfig
				{
					CanBuffer = definition.Execution.CanBuffer,
					StaminaCost = definition.Execution.StaminaCost,
					BufferWindow = definition.Execution.BufferWindow.ToString(),
				},
				Motor = CreateMotorConfig(definition.Motor),
				CancelPolicy = CreateCancelPolicyConfig(definition.CancelPolicy),
			};
		}

		private static PlayerActionMotorProfileConfig CreateMotorConfig(
			PlayerActionMotorProfile motor
		)
		{
			if (motor.Mode == PlayerActionMotorMode.None)
				return null;

			return new PlayerActionMotorProfileConfig
			{
				Mode = motor.Mode.ToString(),
				Phase = motor.Phase.ToString(),
				MoveMultiplier = motor.MoveMultiplier,
			};
		}

		private static PlayerActionCancelPolicyConfig CreateCancelPolicyConfig(
			PlayerActionCancelPolicy cancelPolicy
		)
		{
			if (cancelPolicy.AllowedTargetIds.Length == 0)
				return null;

			return new PlayerActionCancelPolicyConfig
			{
				Window = cancelPolicy.Window.ToString(),
				AllowedTargetIds = CreateTargetIds(cancelPolicy.AllowedTargetIds),
			};
		}

		private static List<string> CreateTargetIds(PlayerActionId[] targetIds)
		{
			var list = new List<string>(targetIds.Length);
			for (int i = 0; i < targetIds.Length; i++)
			{
				list.Add(targetIds[i].ToString());
			}

			return list;
		}

		private static bool TryParseActionConfig(
			PlayerActionDefinitionConfig actionConfig,
			int index,
			HashSet<PlayerActionId> idsInCatalog,
			List<string> errors,
			out ParsedActionEntry parsedEntry
		)
		{
			parsedEntry = default;
			var location = $"Action entry #{index + 1}";
			var startErrorCount = errors.Count;

			if (actionConfig == null)
			{
				errors.Add($"{location} is null.");
				return false;
			}

			if (
				!TryParseIdentity(
					actionConfig,
					location,
					idsInCatalog,
					errors,
					out ParsedIdentity identity
				)
			)
			{
				return false;
			}

			if (!ValidateRequiredSections(actionConfig, location, errors))
				return false;

			var timing = ParseTiming(actionConfig.Timing, location, errors);
			var execution = ParseExecution(actionConfig.Execution, location, errors);
			var motor = ParseMotorProfile(actionConfig.Motor, location, errors);
			var cancel = ParseCancelPolicy(actionConfig.CancelPolicy, location, errors);

			if (errors.Count > startErrorCount)
				return false;

			parsedEntry = new ParsedActionEntry
			{
				Id = identity.Id,
				Source = identity.Source,
				Category = identity.Category,
				AnimationTrigger = identity.AnimationTrigger,
				Timing = timing,
				Availability = new ParsedAvailability
				{
					RequiresGrounded = actionConfig.Availability.RequiresGrounded,
					AllowWhileAirborne = actionConfig.Availability.AllowWhileAirborne,
				},
				Execution = execution,
				Motor = motor,
				Cancel = cancel,
			};

			return true;
		}

		private static bool TryParseIdentity(
			PlayerActionDefinitionConfig actionConfig,
			string location,
			HashSet<PlayerActionId> idsInCatalog,
			List<string> errors,
			out ParsedIdentity identity
		)
		{
			identity = default;

			if (
				!TryParseRequiredEnum(
					actionConfig.Id,
					location + " Id",
					delegate(PlayerActionId value)
					{
						return value != PlayerActionId.None;
					},
					"'None' is not a valid action id.",
					out PlayerActionId id,
					errors
				)
			)
			{
				return false;
			}

			if (!idsInCatalog.Add(id))
			{
				errors.Add(location + " has duplicate action id '" + id + "'.");
				return false;
			}

			var parsed =
				TryParseRequiredEnum(
					actionConfig.Source,
					location + " Source",
					AlwaysValid,
					string.Empty,
					out PlayerActionSource source,
					errors
				)
				& TryParseRequiredEnum(
					actionConfig.Category,
					location + " Category",
					AlwaysValid,
					string.Empty,
					out PlayerActionCategory category,
					errors
				)
				& TryParseRequiredEnum(
					actionConfig.AnimationTrigger,
					location + " AnimationTrigger",
					AlwaysValid,
					string.Empty,
					out AnimTrigger animationTrigger,
					errors
				);

			if (parsed)
			{
				identity = new ParsedIdentity
				{
					Id = id,
					Source = source,
					Category = category,
					AnimationTrigger = animationTrigger,
				};
			}

			return parsed;
		}

		private static bool ValidateRequiredSections(
			PlayerActionDefinitionConfig actionConfig,
			string location,
			List<string> errors
		)
		{
			var valid = true;

			if (actionConfig.Timing == null)
			{
				errors.Add(location + " Timing is required.");
				valid = false;
			}

			if (actionConfig.Availability == null)
			{
				errors.Add(location + " Availability is required.");
				valid = false;
			}

			if (actionConfig.Execution == null)
			{
				errors.Add(location + " Execution is required.");
				valid = false;
			}

			return valid;
		}

		private static ParsedTiming ParseTiming(
			PlayerActionTimingConfig timingConfig,
			string location,
			List<string> errors
		)
		{
			var timing = new ParsedTiming
			{
				StartupSeconds = timingConfig.StartupSeconds,
				ActiveSeconds = timingConfig.ActiveSeconds,
				RecoverySeconds = timingConfig.RecoverySeconds,
			};

			if (timing.StartupSeconds < 0f)
				errors.Add(location + " StartupSeconds must be >= 0.");

			if (timing.ActiveSeconds < 0f)
				errors.Add(location + " ActiveSeconds must be >= 0.");

			if (timing.RecoverySeconds < 0f)
				errors.Add(location + " RecoverySeconds must be >= 0.");

			if ((timing.StartupSeconds + timing.ActiveSeconds + timing.RecoverySeconds) <= 0f)
				errors.Add(location + " total action duration must be > 0.");

			return timing;
		}

		private static ParsedExecution ParseExecution(
			PlayerActionExecutionPolicyConfig executionConfig,
			string location,
			List<string> errors
		)
		{
			var execution = new ParsedExecution
			{
				CanBuffer = executionConfig.CanBuffer,
				StaminaCost = executionConfig.StaminaCost,
			};

			TryParseRequiredEnum(
				executionConfig.BufferWindow,
				location + " Execution.BufferWindow",
				AlwaysValid,
				string.Empty,
				out execution.BufferWindow,
				errors
			);

			if (!execution.CanBuffer && execution.BufferWindow != PlayerActionBufferWindow.None)
			{
				errors.Add(location + " cannot specify a buffer window when CanBuffer is false.");
			}

			if (execution.CanBuffer && execution.BufferWindow == PlayerActionBufferWindow.None)
			{
				errors.Add(
					location + " must specify a non-None buffer window when CanBuffer is true."
				);
			}

			return execution;
		}

		private static ParsedCancelPolicy ParseCancelPolicy(
			PlayerActionCancelPolicyConfig cancelConfig,
			string location,
			List<string> errors
		)
		{
			var cancel = new ParsedCancelPolicy
			{
				Window = PlayerActionBufferWindow.None,
				AllowedTargetIds = Array.Empty<PlayerActionId>(),
			};

			if (cancelConfig == null)
				return cancel;

			TryParseRequiredEnum(
				cancelConfig.Window,
				location + " CancelPolicy.Window",
				delegate(PlayerActionBufferWindow value)
				{
					return value != PlayerActionBufferWindow.None;
				},
				"'None' is not a valid cancel window.",
				out cancel.Window,
				errors
			);

			if (cancelConfig.AllowedTargetIds == null || cancelConfig.AllowedTargetIds.Count == 0)
			{
				errors.Add(location + " CancelPolicy must specify at least one allowed target id.");
				return cancel;
			}

			cancel.AllowedTargetIds = ParseCancelTargets(
				cancelConfig.AllowedTargetIds,
				location,
				errors
			);
			return cancel;
		}

		private static ParsedMotor ParseMotorProfile(
			PlayerActionMotorProfileConfig motorConfig,
			string location,
			List<string> errors
		)
		{
			var motor = new ParsedMotor
			{
				Mode = PlayerActionMotorMode.None,
				Phase = PlayerActionPhase.None,
				MoveMultiplier = 0f,
			};

			if (motorConfig == null)
				return motor;

			TryParseRequiredEnum(
				motorConfig.Mode,
				location + " Motor.Mode",
				delegate(PlayerActionMotorMode value)
				{
					return value != PlayerActionMotorMode.None;
				},
				"'None' is not a valid motor mode.",
				out motor.Mode,
				errors
			);

			TryParseRequiredEnum(
				motorConfig.Phase,
				location + " Motor.Phase",
				delegate(PlayerActionPhase value)
				{
					return value != PlayerActionPhase.None;
				},
				"'None' is not a valid motor phase.",
				out motor.Phase,
				errors
			);

			if (motorConfig.MoveMultiplier <= 0f)
			{
				errors.Add(location + " Motor.MoveMultiplier must be > 0.");
			}
			else
			{
				motor.MoveMultiplier = motorConfig.MoveMultiplier;
			}

			return motor;
		}

		private static PlayerActionId[] ParseCancelTargets(
			List<string> targetIds,
			string location,
			List<string> errors
		)
		{
			var parsedTargetIds = new PlayerActionId[targetIds.Count];

			for (int i = 0; i < targetIds.Count; i++)
			{
				TryParseRequiredEnum(
					targetIds[i],
					location + " CancelPolicy.AllowedTargetIds[" + i + "]",
					delegate(PlayerActionId value)
					{
						return value != PlayerActionId.None;
					},
					"'None' is not a valid cancel target id.",
					out parsedTargetIds[i],
					errors
				);
			}

			return parsedTargetIds;
		}

		private static bool TryParseRequiredEnum<TEnum>(
			string rawValue,
			string label,
			Func<TEnum, bool> additionalValidation,
			string additionalValidationMessage,
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

			if (!additionalValidation(value))
			{
				errors.Add(
					label + " value '" + rawValue + "' is invalid. " + additionalValidationMessage
				);
				return false;
			}

			return true;
		}

		private static bool AlwaysValid<T>(T _)
		{
			return true;
		}

		private static PlayerActionDefinitionRegistry CreateEmptyRegistry()
		{
			return new PlayerActionDefinitionRegistry(Array.Empty<PlayerActionDefinition>());
		}

		private struct ParsedActionEntry
		{
			public PlayerActionId Id;
			public PlayerActionSource Source;
			public PlayerActionCategory Category;
			public AnimTrigger AnimationTrigger;
			public ParsedTiming Timing;
			public ParsedAvailability Availability;
			public ParsedExecution Execution;
			public ParsedMotor Motor;
			public ParsedCancelPolicy Cancel;
		}

		private struct ParsedTiming
		{
			public float StartupSeconds;
			public float ActiveSeconds;
			public float RecoverySeconds;
		}

		private struct ParsedIdentity
		{
			public PlayerActionId Id;
			public PlayerActionSource Source;
			public PlayerActionCategory Category;
			public AnimTrigger AnimationTrigger;
		}

		private struct ParsedAvailability
		{
			public bool RequiresGrounded;
			public bool AllowWhileAirborne;
		}

		private struct ParsedExecution
		{
			public bool CanBuffer;
			public float StaminaCost;
			public PlayerActionBufferWindow BufferWindow;
		}

		private struct ParsedMotor
		{
			public PlayerActionMotorMode Mode;
			public PlayerActionPhase Phase;
			public float MoveMultiplier;
		}

		private struct ParsedCancelPolicy
		{
			public PlayerActionBufferWindow Window;
			public PlayerActionId[] AllowedTargetIds;
		}
	}
}
