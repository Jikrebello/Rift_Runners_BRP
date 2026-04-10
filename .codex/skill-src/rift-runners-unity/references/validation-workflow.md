# Validation Workflow

## Validation Categories

### Core / Shared / Tests
Use this when changes are limited to:
- `Assets/Scripts/Game/Characters/Core/**`
- `Assets/Scripts/Game/SharedKernel/**`
- `Assets/Tests/EditMode/**`
- mirror-project or test-project files under `src/**` and `tests/**`

Command:

```powershell
dotnet test RiftRunners.Core.slnx --no-restore
```

This validates the current solution-backed mirror projects and test project.

## Freshly Verified During This Implementation
The command above was rerun during this change set and completed successfully.

Observed result from that fresh run:
- 125 passed
- 0 failed
- 0 skipped

No warning baseline is frozen here. The captured output from this docs-only implementation rerun did not emit compiler warnings, so do not infer a stable warning-free baseline from this file alone.

### Unity Wrapper Code
Changes under `Assets/Scripts/Game/Characters/Unity/**` are not proven by `dotnet test RiftRunners.Core.slnx --no-restore`.

If you need Unity-side validation, use a real Unity batchmode run from a locally installed editor that matches `ProjectSettings/ProjectVersion.txt`.

Example shape only:

```powershell
& $UnityEditorPath `
	-batchmode `
	-projectPath $ProjectPath `
	-runTests `
	-testPlatform EditMode `
	-testResults $ResultsPath `
	-logFile $LogPath `
	-quit
```

Do not treat that example as verified until you confirm:
- the editor path is valid on the current machine
- the run completes
- the expected result artifact is produced

## Fresh Unity Validation Status During This Implementation
- A local Unity 6000.3.10f1 editor installation was discovered.
- A batchmode launch was attempted.
- The editor started and wrote a log, but the expected test result XML was not produced during this session.
- Because that end-to-end result was not verified, Unity-wrapper validation remains unverified for this implementation.

### Documentation-Only Changes
- Repo docs, skill docs, templates, and Serena/Codex config changes do not require gameplay test coverage.
- Still verify scripts and config structure when applicable.

## Script and Skill Validation
- Run the repo install script multiple times to confirm idempotent sync behavior.
- Run it once with `-Force` to confirm full replacement behavior.
- Validate the repo skill structure with the local skill validator if available in the current environment.
