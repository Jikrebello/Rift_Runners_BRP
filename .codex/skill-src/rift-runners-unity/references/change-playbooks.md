# Change Playbooks

## Add a Traversal Rule
- Code placement:
  - Traversal semantics, states, and transitions go under `Assets/Scripts/Game/Characters/Core/Player/Traversal`.
  - Unity sensing/building of world snapshots stays under `Assets/Scripts/Game/Characters/Unity/Player/World`.
- Keep out of Unity:
  - transition legality
  - stamina costs
  - state-machine decisions
- Tests:
  - add or update traversal-focused tests in `Assets/Tests/EditMode`
- Validation:
  - `dotnet test RiftRunners.Core.slnx --no-restore`
  - if Unity wrapper inputs/world snapshots changed, Unity validation is also needed
- Traps:
  - letting motor implementation details leak into core
  - hardcoding tuning values inside Unity adapters

## Add an Intent / Action Mapping
- Code placement:
  - input snapshots in `Core/Player/Input`
  - intent meaning/arbitration in `Core/Player/Intent`
  - action definitions/resolution/runtime in `Core/Player/Action`
  - Unity input plumbing only in `Unity/Player/Input`
- Keep out of Unity:
  - context meaning
  - arbitration priority
  - legality checks
- Tests:
  - add or update intent resolution or action resolution tests in `Assets/Tests/EditMode`
- Validation:
  - `dotnet test RiftRunners.Core.slnx --no-restore`
  - Unity validation only if adapter plumbing changed
- Traps:
  - burying semantic rules inside button-reader code
  - representing gameplay meaning with raw strings

## Change Stamina or Exhaustion Behavior
- Code placement:
  - rule changes belong in `Core/Player/Resources/Stamina`
  - content/tuning values should move into owning config/value objects rather than inline literals
  - Unity-side display/binding changes belong in `Unity/Player/Resources`
- Keep out of Unity:
  - drain policy
  - regen policy
  - exhaustion state decisions
- Tests:
  - add or update stamina/exhaustion tests in `Assets/Tests/EditMode`
- Validation:
  - `dotnet test RiftRunners.Core.slnx --no-restore`
  - Unity validation only if wrapper binding/UI behavior changed
- Traps:
  - duplicating costs across action and traversal code
  - hiding gameplay meaning in scattered numeric literals

## Change Unity Host / Adapter Behavior Safely
- Code placement:
  - keep it under the owning Unity folder: `Host`, `Input`, `Motor`, `Anim`, or `World`
  - if the change adds rules, first push those rules into core
- Keep out of Unity:
  - policy decisions that can be expressed as pure logic
  - duplicated state that should live in the core model
- Tests:
  - add or update core tests if logic was pushed down
  - if the change cannot be pushed down, document the remaining untested adapter surface
- Validation:
  - run `dotnet test RiftRunners.Core.slnx --no-restore` for any affected core logic
  - do not claim Unity-wrapper correctness without a verified Unity-side run
- Traps:
  - MonoBehaviour bloat
  - direct scene-object coupling where a snapshot/output adapter would do

## Move Hardcoded Values Into Config / Constants
- Code placement:
  - local fixed policy: named constant near the owning type
  - tunable gameplay data: owning config/value object in the slice that owns the rule
  - Unity-exposed tuning: Unity config type under the relevant `Unity/**/Config` folder, then map into core-friendly config
- Keep out of Unity:
  - gameplay-only values that do not need inspector/config-asset ownership
- Tests:
  - update tests to assert behavior through the new config/constants path
- Validation:
  - `dotnet test RiftRunners.Core.slnx --no-restore`
- Traps:
  - dumping unrelated values into a generic constants file
  - keeping both the old literal and the new config path alive
