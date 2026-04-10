---
name: rift-runners-unity
description: Repo-scoped guidance for working in the Rift_Runners_BRP Unity project. Use when Codex needs to understand or modify this repository's core gameplay code, Unity wrapper code, shared kernel code, tests, or project architecture while preserving the existing core-versus-Unity separation and repo-specific validation workflow.
---

# Rift Runners Unity

Start with `AGENTS.md`, then read only the specific reference docs you need from `references/`.

## Working Rules
- Use symbol-aware navigation for C# code when you know the symbol or file area.
- Use text search for markdown, config, unknown symbol discovery, or when you need to find a concept across docs.
- Keep core and shared code free of `UnityEngine` and Unity-only APIs.
- Keep Unity code thin. Push testable logic into core whenever practical.
- Place new code in the existing vertical slice and subdomain. Do not create catch-all folders.
- Add or update tests alongside any testable core change.
- Prefer named constants, config objects, enums, value objects, or explicit lookup mappings over new magic numbers or strings.
- Treat the docs in `Assets/Documentation/PDF` as canonical context and keep summaries secondary to them.

## Reference Guide
- Repo map and current structure: `references/repo-map.md`
- Boundary and placement rules: `references/architecture-boundaries.md`
- Validation split and evidence rules: `references/validation-workflow.md`
- Common implementation playbooks: `references/change-playbooks.md`
- Canonical source doc index: `references/source-doc-index.md`
