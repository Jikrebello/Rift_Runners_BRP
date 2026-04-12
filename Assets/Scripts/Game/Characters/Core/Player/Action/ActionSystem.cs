using Assets.Scripts.Game.Characters.Core.Player.Action.Definitions;
using Assets.Scripts.Game.Characters.Core.Player.Action.Resolution;
using Assets.Scripts.Game.Characters.Core.Player.Action.Runtime;
using Assets.Scripts.Game.Characters.Core.Player.Model;
using Assets.Scripts.Game.Characters.Core.Player.Outputs;

namespace Assets.Scripts.Game.Characters.Core.Player.Action
{
	public sealed class ActionSystem
	{
		public void HandleResolvedAction(
			PlayerModel model,
			PlayerOutputs outputs,
			ResolvedPlayerActionRequest? request
		)
		{
			if (!request.HasValue)
				return;

			var requested = request.Value.Action;

			if (!CanStart(model, requested))
			{
				outputs.Debug.Info("Action", $"Rejected action start: {requested.Id}.");
				return;
			}

			if (!model.ActionRuntime.HasActiveAction)
			{
				StartAction(model, outputs, requested);
				return;
			}

			var current = PlayerActionDefinitions.Get(model.ActionRuntime.CurrentActionId);

			if (TryCancelIntoRequestedAction(model, outputs, current, requested))
				return;

			TryBufferAction(model, outputs, current, requested);
		}

		public void Tick(PlayerModel model, PlayerOutputs outputs, float dt)
		{
			TickCurrentAction(model, outputs, dt);
		}

		private static bool CanStart(PlayerModel model, PlayerActionDefinition action)
		{
			if (
				action.Availability.RequiresGrounded
				&& model.TraversalMode != PlayerTraversalMode.Grounded
			)
				return false;

			if (
				!action.Availability.AllowWhileAirborne
				&& model.TraversalMode == PlayerTraversalMode.Airborne
			)
				return false;

			return true;
		}

		private static void TryBufferAction(
			PlayerModel model,
			PlayerOutputs outputs,
			PlayerActionDefinition current,
			PlayerActionDefinition requested
		)
		{
			if (model.ActionRuntime.HasBufferedAction)
				return;

			if (!current.Execution.CanBuffer)
			{
				outputs.Debug.Info(
					"Action",
					$"Rejected buffer: current action cannot buffer ({current.Id})."
				);
				return;
			}

			if (!current.Execution.AllowsBufferFrom(model.ActionRuntime.CurrentPhase))
			{
				outputs.Debug.Info(
					"Action",
					$"Rejected buffer: current phase does not allow buffering ({current.Id}, {model.ActionRuntime.CurrentPhase})."
				);
				return;
			}

			model.ActionRuntime.BufferedRequestedActionId = requested.Id;
			outputs.Debug.Info("Action", $"Buffered requested action: {requested.Id}.");
		}

		private static bool TryCancelIntoRequestedAction(
			PlayerModel model,
			PlayerOutputs outputs,
			PlayerActionDefinition current,
			PlayerActionDefinition requested
		)
		{
			var currentPhase = model.ActionRuntime.CurrentPhase;
			if (!current.CancelPolicy.AllowsCancelTo(requested.Id, currentPhase))
				return false;

			var currentActionId = current.Id;
			model.ActionRuntime.ClearCurrent();
			model.ActionRuntime.ClearBuffered();

			outputs.Debug.Info(
				"Action",
				$"Cancelled action: {currentActionId} -> {requested.Id} during {currentPhase}."
			);

			StartAction(model, outputs, requested);
			return true;
		}

		private static void StartAction(
			PlayerModel model,
			PlayerOutputs outputs,
			PlayerActionDefinition action
		)
		{
			model.ActionRuntime.CurrentActionId = action.Id;
			model.ActionRuntime.CurrentPhase = PlayerActionPhase.Startup;
			model.ActionRuntime.PhaseElapsedSeconds = 0f;

			outputs.Animation.AddTrigger(action.AnimationTrigger);
			outputs.Debug.Info("Action", $"Started action: {action.Id} -> Startup.");
		}

		private static void TickCurrentAction(PlayerModel model, PlayerOutputs outputs, float dt)
		{
			if (!model.ActionRuntime.HasActiveAction)
				return;

			model.ActionRuntime.PhaseElapsedSeconds += dt;

			var action = PlayerActionDefinitions.Get(model.ActionRuntime.CurrentActionId);

			switch (model.ActionRuntime.CurrentPhase)
			{
				case PlayerActionPhase.Startup:
					if (model.ActionRuntime.PhaseElapsedSeconds >= action.Timing.StartupSeconds)
					{
						AdvanceToPhase(model, outputs, PlayerActionPhase.Active);
					}
					break;

				case PlayerActionPhase.Active:
					if (model.ActionRuntime.PhaseElapsedSeconds >= action.Timing.ActiveSeconds)
					{
						AdvanceToPhase(model, outputs, PlayerActionPhase.Recovery);
					}
					break;

				case PlayerActionPhase.Recovery:
					if (model.ActionRuntime.PhaseElapsedSeconds >= action.Timing.RecoverySeconds)
					{
						var completedActionId = model.ActionRuntime.CurrentActionId;

						outputs.Debug.Info("Action", $"Completed action: {completedActionId}.");

						model.ActionRuntime.ClearCurrent();
						TryPromoteBufferedAction(model, outputs, completedActionId);
					}
					break;
			}
		}

		private static void AdvanceToPhase(
			PlayerModel model,
			PlayerOutputs outputs,
			PlayerActionPhase nextPhase
		)
		{
			model.ActionRuntime.CurrentPhase = nextPhase;
			model.ActionRuntime.PhaseElapsedSeconds = 0f;

			outputs.Debug.Info("Action", $"Action phase -> {nextPhase}.");
		}

		private static void TryPromoteBufferedAction(
			PlayerModel model,
			PlayerOutputs outputs,
			PlayerActionId completedActionId
		)
		{
			if (!model.ActionRuntime.HasBufferedAction)
				return;

			var requestedId = model.ActionRuntime.BufferedRequestedActionId;
			model.ActionRuntime.ClearBuffered();

			var nextActionId = PlayerActionGrammar.ResolveFollowUp(completedActionId, requestedId);

			var nextAction = PlayerActionDefinitions.Get(nextActionId);
			if (nextAction.Id == PlayerActionId.None)
				return;

			StartAction(model, outputs, nextAction);
		}
	}
}
