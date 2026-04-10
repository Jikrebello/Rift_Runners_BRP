# Rift Runners Agent Map

Start here, then use the deeper references under `.codex/skill-src/rift-runners-unity/references/`.

## Repo Map
- Core gameplay/domain logic: `Assets/Scripts/Game/Characters/Core`
- Unity adapter/host code: `Assets/Scripts/Game/Characters/Unity`
- Shared cross-cutting domain code: `Assets/Scripts/Game/SharedKernel`
- Unity EditMode tests: `Assets/Tests/EditMode`
- .NET mirror projects: `src/RiftRunners.Game.Characters.Core`, `src/RiftRunners.Game.SharedKernel`
- .NET test project: `tests/RiftRunners.Game.Characters.Core.Tests`
- Canonical architecture/design docs: `Assets/Documentation/PDF`

## Non-Negotiable Rules
- Do not reference `UnityEngine` or Unity-only APIs from `Assets/Scripts/Game/Characters/Core` or `Assets/Scripts/Game/SharedKernel`.
- Treat `Assets/Scripts/Game/Characters/Unity` as adapter, host, presentation, and application glue only.
- Core decides what should happen. Unity decides how to gather inputs, build snapshots, and apply outputs.
- Put new code in the correct vertical slice and subdomain. Do not create vague folders such as `misc`, `helpers`, or `utils`.
- Prefer explicit names, small focused types, narrow interfaces, composition, and named constants/config over new magic values.
- Push testable logic into core whenever practical so it can be validated outside Unity.

## Validation
- Core/shared/tests validation: `dotnet test RiftRunners.Core.slnx --no-restore`
- That command validates the mirror projects and `Assets/Tests/EditMode` as compiled by the .NET test project.
- That command does not by itself validate `Assets/Scripts/Game/Characters/Unity/**`.
- Documentation-only changes do not require code validation.

## Working Pattern
- Use the docs in `Assets/Documentation/PDF` as canonical design context unless the code clearly disagrees.
- Keep summaries secondary to those source docs; do not restate them wholesale.
- For deeper repo, boundary, playbook, and validation guidance, use `.codex/skill-src/rift-runners-unity/references/`.
