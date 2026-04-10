# Codex Repo Guidance

This folder holds repo-scoped guidance for Codex. The repository is the source of truth.

## What Lives Here
- `install-rift-runners-skill.ps1`: syncs the repo skill source into the user Codex skills folder for discovery
- `templates/work-item.md`: short task template for future work
- `skill-src/rift-runners-unity/`: repo-local skill source and reference docs

## How To Use It
1. Read `AGENTS.md` for the repo map and hard rules.
2. Use the reference docs under `.codex/skill-src/rift-runners-unity/references/` for deeper guidance.
3. If you want Codex to auto-discover the project skill, run:

```powershell
powershell -ExecutionPolicy Bypass -File .\.codex\install-rift-runners-skill.ps1
```

The installed skill is a synced copy. Update the repo copy first, then re-run the install script.
