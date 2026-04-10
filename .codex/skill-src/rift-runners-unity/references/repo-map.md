# Repo Map

## Runtime and Domain Code
- `Assets/Scripts/Game/Characters/Core`
  - Pure gameplay and domain logic for the character/player slice.
  - Current subdomains include `Action`, `CombatMode`, `Input`, `Intent`, `Model`, `Outputs`, `Resources/Stamina`, and `Traversal`.
- `Assets/Scripts/Game/Characters/Unity`
  - Unity-only adapters, hosts, config assets, input readers, motor adapters, animation adapters, and world snapshot builders.
- `Assets/Scripts/Game/SharedKernel`
  - Shared cross-cutting domain code such as math and config loading primitives that are still meant to stay engine-agnostic.

## Tests and Mirrors
- `Assets/Tests/EditMode`
  - Canonical EditMode test sources.
- `src/RiftRunners.Game.Characters.Core`
  - .NET mirror project that compiles `Assets/Scripts/Game/Characters/Core/**/*.cs`.
- `src/RiftRunners.Game.SharedKernel`
  - .NET mirror project that compiles `Assets/Scripts/Game/SharedKernel/**/*.cs`.
- `tests/RiftRunners.Game.Characters.Core.Tests`
  - .NET test project that compiles `Assets/Tests/EditMode/**/*.cs`.
- `RiftRunners.Core.slnx`
  - Solution containing the current core, shared-kernel, and test mirror projects only.

## Project Context
- `Assets/Documentation/PDF`
  - Canonical architecture, progress, and implementation-context docs. Prefer the `.md` files when a `.md` and `.pdf` pair both exist.
- `Packages`
  - Unity package manifest and package state.
- `ProjectSettings`
  - Unity project configuration, including `ProjectVersion.txt`.
- `Configs`
  - Project-local configuration assets/files outside the code folders.

## Validation Implication
- The .NET mirror/test path validates core/shared/tests compiled from `Assets/**`.
- Changes under `Assets/Scripts/Game/Characters/Unity/**` are outside the scope of `dotnet test RiftRunners.Core.slnx --no-restore`.
