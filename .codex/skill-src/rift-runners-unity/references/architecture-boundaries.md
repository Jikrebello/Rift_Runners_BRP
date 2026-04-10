# Architecture Boundaries

## Core Boundary
- `Assets/Scripts/Game/Characters/Core` and `Assets/Scripts/Game/SharedKernel` must stay free of `UnityEngine` and Unity-only APIs.
- Core and shared should define decisions, rules, state transitions, value types, outputs, and engine-agnostic contracts.
- Unity code should gather inputs, build snapshots, bind config assets, and apply outputs. It should not become the place where gameplay rules live.

## Placement Rules
| Area | Put It Here | Keep Out |
| --- | --- | --- |
| Intent meaning and arbitration | `Core/Player/Intent` | Unity input readers and MonoBehaviours |
| Raw action-state snapshots | `Core/Player/Input` | Unity input callbacks with engine-specific types |
| Durable player state | `Core/Player/Model` | Animation, scene references, `Transform` access |
| Output contracts | `Core/Player/Outputs` | Unity animation or motor application details |
| Traversal rules and state transitions | `Core/Player/Traversal` | CharacterController or Rigidbody calls |
| Action definitions, resolution, runtime | `Core/Player/Action` | Animation trigger wiring |
| Combat readiness / mode state | `Core/Player/CombatMode` | Scene or inspector wiring |
| Stamina/exhaustion rules | `Core/Player/Resources/Stamina` | Unity UI or VFX handling |
| Unity host composition | `Unity/Player/Host` | Domain rules or policy decisions |
| Unity input adapters | `Unity/Player/Input` | Intent meaning or combat legality |
| Unity motor integration | `Unity/Player/Motor` | Traversal rules |
| Unity animation adapters | `Unity/Player/Anim` | Action legality or timing policy |
| Unity world snapshot building | `Unity/Player/World` | Persistent domain state |
| Cross-cutting engine-agnostic primitives | `SharedKernel` | Game-specific feature dumping |

## Design Expectations
- Follow SOLID, Clean Code, and DRY, but do not introduce abstractions unless they solve a real ownership or dependency problem.
- Prefer explicit names, focused types, narrow interfaces, and composition over god objects or “manager” blobs.
- Do not create junk-drawer folders such as `misc`, `helpers`, or `utils`. Name folders after real domain ownership.

## Hardcoded Value Policy
- Do not add new magic numbers or strings when the value has gameplay or integration meaning.
- Prefer one of:
  - named constants for small fixed policies local to a type
  - config objects when values are meant to vary by tuning or content
  - enums or value objects when a raw primitive hides meaning
  - explicit lookup mappings when behavior varies by mode, action, slot, or state
- Keep constants/config close to the owning slice. Do not centralize unrelated values into generic constants files.

## Testing Direction
- If logic is testable in core, keep it in core and cover it in `Assets/Tests/EditMode` through the .NET mirror test path.
- If a requested change begins in Unity wrappers, first ask whether the decision-making can be pushed down into core so it becomes testable.
