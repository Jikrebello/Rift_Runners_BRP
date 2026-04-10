# Serena Onboarding

## Recommended Use
- Activate this repo in Serena and let the C# language server index the project before doing symbol-heavy edits.
- Prefer symbol-aware navigation first for C# code:
  - symbol overview when entering a file
  - symbol lookup when you know the type or member
  - reference lookup when you need impact analysis
- Prefer text search when:
  - you are searching markdown or config
  - you do not know the symbol name yet
  - you are locating a concept across docs and code

## Repo Expectations
- Personal Serena caches, memories, and local overrides stay outside the repo.
- This repo only commits the minimal project-level Serena config and onboarding note.
- If symbols look stale after non-Serena edits, restart the language server before trusting navigation results.
