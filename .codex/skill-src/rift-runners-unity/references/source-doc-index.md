# Source Doc Index

Use the docs under `Assets/Documentation/PDF` as canonical context. Prefer the markdown file when a markdown and PDF pair both exist.

## Strategic Architecture
- `Assets/Documentation/PDF/Game Architecture Design Document.md`
  - High-level system boundaries, module ownership, repository direction, and persistence/world architecture.
- `Assets/Documentation/PDF/PlayerArchitecture.md`
  - Player-control pipeline, domain split, intent flow, and action/combat architecture.
- `Assets/Documentation/PDF/Persistence.md`
  - Durable-versus-transient save guidance, IDs, save structure, and migration/scaling direction.

## Progress / Status
- `Assets/Documentation/PDF/Current_Progress_Milestone.md`
  - Snapshot of how close the current combat/action architecture is to the intended data-driven direction and which layers still need to land.

## Implementation Notes
- `Assets/Documentation/PDF/Pickup from old chat.md`
  - Advisory implementation notes about the intended DDDA-like combat direction, gaps in the current action model, and next architectural steps. Useful context, but less canonical than the strategic architecture docs above.

## Additional Architecture Context
- `Assets/Documentation/PDF/rift_runners_architecture_handoff.pdf`
  - Supplemental handoff context.
- `Assets/Documentation/PDF/rift_runners_cancel_system_mvp.pdf`
  - Additional cancel-system design context.
- `Assets/Documentation/PDF/rift_runners_consolidated_resume_sheet_and_addenda.pdf`
  - Additional consolidated planning/handoff context.
- `Assets/Documentation/PDF/rift_runners_data_driven_action_layer.pdf`
  - Additional data-driven action-layer context.

## Usage Notes
- Start with the strategic architecture docs for boundaries and ownership.
- Use the progress and implementation-note docs to understand current gaps, sequencing, and interim architectural intent.
- If code and docs disagree, inspect the code carefully and call out the mismatch explicitly instead of silently picking one.
