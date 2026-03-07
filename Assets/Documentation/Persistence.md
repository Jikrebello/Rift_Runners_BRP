# Persistence.md

## 1. Purpose

This document specifies the persistence architecture for:
- Saving/loading player state (stats, equipment, skill loadouts, reputation)
- Quest journal and objective progress
- Phase-based world variants (Autumn/Winter/Spring) with **per-phase deltas**
- Selective cross-phase continuity using **WorldObjectId**
- GameMaster state (scheduled tasks, world metrics, cooldowns)
- Versioning and migrations

The intent is to support:
- Simple JSON-based saves early
- Seamless evolution to SQLite/chunked persistence for large open worlds later
- Engine portability (Unity now, Stride later)

---

## 2. Fundamental Principle: Save durable facts, not runtime mechanics

### 2.1 Durable (save)
- Player model:
  - stats (int/wis/cha/luck, etc.)
  - reputation (sins/virtues, fame/infamy, faction standings)
  - inventory/equipment
  - learned skills + skill slot loadouts
  - resources (HP/stamina/mana) if designed to persist
- Quest journal:
  - quest status (seen/accepted/rejected/completed/failed)
  - stage id
  - objective progress
  - quest flags
- World state:
  - `WorldPhase` (Autumn/Winter/Spring)
  - global flags (story booleans)
  - per-phase cell deltas (differences from baseline content of that phase)
  - global `WorldObjectState` keyed by `WorldObjectId`
- GameMaster:
  - scheduled tasks queue
  - world metrics (bandit pressure, region security)
  - rule cooldowns

### 2.2 Transient (do not save)
- FSM state instances / class objects
- Animation graph runtime progress
- Input buffers unless explicitly designed
- Cached raycasts or proximity candidates
- Event subscriptions

### 2.3 Load policy
On load, rebuild runtime systems from durable state:
- Create `PlayerPiece` from `PlayerModel`
- Load quests into `QuestJournal`
- Load world baseline for current phase, then apply deltas
- Restore GM scheduler state

---

## 3. Key Concepts & IDs

### 3.1 IDs (stable, engine-agnostic)
All saved references are ID-based:
- `EntityId` / `PhaseEntityId` (phase-scoped runtime entities)
- `WorldObjectId` (selective cross-phase continuity objects)
- `CellId` (world partition unit)
- `QuestId`, `ObjectiveId`
- `SkillId`, `WeaponId`, `ItemId`
- `FactionId`, `RegionId`

**Rule:** never store engine instance IDs or scene object references in saves.

### 3.2 WorldPhase
- `Autumn`, `Winter`, `Spring`
- Each phase has its own baseline world content.

### 3.3 Baseline vs delta
- Baseline content is loaded from `Game.Content` for the active phase.
- Deltas store only differences from baseline.

---

## 4. Storage Patterns

Two patterns are supported. Start with A; migrate to B if needed.

### 4.1 Pattern A: JSON save file (recommended early)
- Entire save is one JSON document
- Atomic writes: write `.tmp`, then rename
- Compression optional (gzip/zstd)

### 4.2 Pattern B: SQLite / chunked persistence (recommended at scale)
- Store global state + indices in SQLite
- Store large per-cell delta blobs in chunk files (optional hybrid)

This document specifies **schema-stable SaveModel contracts** so storage can change without rewriting gameplay code.

---

## 5. Save Model Structure

### 5.1 Root model
`SaveGame` contains:
- `saveVersion`
- `createdUtc`, `lastSavedUtc`
- `global` (player/quests/world objects/gm)
- `phases` (Autumn/Winter/Spring phase data)

### 5.2 Global section
Stores cross-phase durable state:
- current phase and time
- global flags
- player model
- quest journal
- `WorldObjectState` map
- GameMaster state

### 5.3 Phase sections
Stores per-phase deltas:
- phase time
- per-cell deltas
- (optional) phase-local flags

---

## 6. JSON Schema Example (Illustrative)

> This is an illustrative shape, not a strict schema.

```json
{
  "saveVersion": 3,
  "createdUtc": "2026-02-27T10:00:00Z",
  "lastSavedUtc": "2026-02-27T11:12:43Z",
  "global": {
    "currentPhase": "Winter",
    "globalFlags": {
      "story.banditsDefeated": false,
      "story.bridgeEastDestroyed": true
    },
    "player": {
      "id": "player-1",
      "location": {
        "phaseSpawnPointId": "winter_from_autumn_mountainpass",
        "cellId": "winter:mountainpass_cell_01",
        "position": { "x": 12.4, "y": 0.0, "z": -7.8 },
        "rotationY": 183.0
      },
      "stats": { "int": 12, "wis": 9, "cha": 14, "luck": 6 },
      "resources": { "hp": 87, "stamina": 41, "mana": 10 },
      "equipment": {
        "meleeWeaponId": "wpn_broadsword_01",
        "rangedWeaponId": "wpn_shortbow_02",
        "armorIds": ["arm_leather_01"]
      },
      "skills": {
        "learned": ["sk_slash_01", "sk_parry_01", "sk_multishot_01"],
        "meleeSlots": { "1": "sk_slash_01", "2": "sk_parry_01", "3": "sk_shieldbash_01" },
        "rangedSlots": { "1": "sk_multishot_01", "2": "sk_poisonarrow_01", "3": "sk_crippleshot_01" }
      },
      "reputation": {
        "fame": 12,
        "infamy": 3,
        "factionStanding": {
          "bandits_redjackals": -20,
          "town_guard": 5
        },
        "virtues": { "temperance": 3, "justice": 1 },
        "sins": { "wrath": 7, "greed": 2 }
      }
    },
    "quests": {
      "active": [
        {
          "questId": "q_robbery_001",
          "status": "Accepted",
          "stageId": "stage_2",
          "objectives": { "talkedToVictim": true, "goldPaid": 0 },
          "flags": ["heard_rumor_redjackals"]
        }
      ],
      "completed": ["q_intro_001"],
      "failed": ["q_medicine_001"]
    },
    "worldObjects": {
      "bridge_east_01": { "flags": { "destroyed": true }, "values": {}, "variantTag": "destroyed" },
      "gate_castle_02": { "flags": { "unlocked": true }, "values": {} }
    },
    "gameMaster": {
      "metrics": {
        "regionSecurity.whiterun": 0.62,
        "banditPressure.redjackals": 0.78
      },
      "cooldowns": {
        "BanditRetaliationRule": "2026-02-28T00:00:00Z"
      },
      "tasks": [
        {
          "taskId": "gm_task_001",
          "type": "SpawnAmbush",
          "phaseScope": "Winter",
          "trigger": { "kind": "OnEnterCell", "cellId": "winter:road_cell_03" },
          "payload": { "factionId": "bandits_redjackals", "squadTemplateId": "ambush_small" }
        }
      ]
    }
  },
  "phases": {
    "Autumn": {
      "phaseTime": { "day": 12, "timeOfDay": 0.42 },
      "cellDeltas": {
        "autumn:forest_cell_02": {
          "entityDeltas": {
            "autumn:forest_cell_02:chest_12": { "flags": { "looted": true }, "items": [] }
          }
        }
      }
    },
    "Winter": {
      "phaseTime": { "day": 3, "timeOfDay": 0.18 },
      "cellDeltas": {
        "winter:village_cell_01": {
          "entityDeltas": {
            "winter:village_cell_01:door_05": { "flags": { "unlocked": true } }
          }
        }
      }
    },
    "Spring": {
      "phaseTime": { "day": 1, "timeOfDay": 0.05 },
      "cellDeltas": {}
    }
  }
}
```

------

## 7. Per-Phase Deltas: Rules and Keying

### 7.1 Key format recommendations

- `CellId` includes phase prefix: `winter:village_cell_01`
- `PhaseEntityId` includes phase + cell + local entity name:
  - `winter:village_cell_01:door_05`

### 7.2 What goes into `EntityDelta`

Keep deltas minimal and semantic:

- `enabled/disabled`
- `flags` (unlocked, looted, destroyed, hostile, etc.)
- inventory state for containers
- (optional) transform overrides for moved objects

Avoid saving:

- physics state
- animation state
- runtime caches

### 7.3 Delta application

When a cell loads:

1. load baseline entities from phase content
2. apply deltas:
   - set flags
   - remove/disable entities
   - replace variants (if applicable)
3. apply global `WorldObjectState` for entities that have `WorldObjectId`

------

## 8. WorldObjectId: Selective Cross-Phase Continuity

### 8.1 Use cases

Use `WorldObjectId` only for important continuity objects:

- bridges
- key gates/doors
- major story props
- major NPC fates (optional)

### 8.2 Storage

Stored in `global.worldObjects` (cross-phase):

- flags: boolean facts
- values: numeric facts
- variantTag: optional hint for representation choice

### 8.3 Application policy

Phase baseline content must provide phase-specific representations that bind to the same `WorldObjectId`.
On load, the representation reads `WorldObjectState` and:

- swaps mesh/variant
- sets interaction availability
- blocks traversal, etc.

------

## 9. Quest Persistence

### 9.1 Quest instance state

Per quest:

- `questId`
- `status` (Seen/Accepted/Rejected/Completed/Failed)
- `stageId`
- objective progress map
- quest flags

### 9.2 Phase gating & expiry

Quest definitions include:

- `AvailablePhases`
- `ExpiryPolicy` (end-of-phase, after N days, never)
- `OnPhaseTransition` (fail/convert/freeze)

On phase change, QuestJournal runs a transition pass.

------

## 10. GameMaster Persistence

### 10.1 GM save data includes

- metrics (region security, pressures)
- cooldowns for rules
- scheduled tasks queue

### 10.2 Task model

Each task contains:

- `type`
- `phaseScope` (optional)
- trigger:
  - time-based, on-enter-cell, condition-based
- payload (data-driven parameters)

------

## 11. Save/Load API (Ports)

### 11.1 SaveStore port (storage-agnostic)

`ISaveStore` supports:

- `Load(slot) -> SaveGame`
- `Save(slot, SaveGame)`
- `ListSlots()`

For large open worlds later, evolve to:

- `PutCellDelta(phase, cellId, delta)`
- `GetCellDelta(phase, cellId)`
- `PutWorldObjectState(id, state)`
- `BeginTransaction/Commit`

### 11.2 Serializer port

`ISaveSerializer` supports:

- `Serialize(SaveGame) -> bytes`
- `Deserialize(bytes) -> SaveGame`

------

## 12. Atomic writes and corruption resistance (JSON)

### 12.1 Atomic save procedure

1. Serialize to bytes
2. Write to `slotN.tmp`
3. Flush
4. Rename/replace `slotN.sav`

Optionally:

- keep `slotN.bak` as last good save

------

## 13. Versioning and Migrations

### 13.1 SaveVersion

Every save has `saveVersion`.

### 13.2 Migration runner

On load:

- If saveVersion < current:
  - run sequential migrations: v1→v2→v3
- Migrations must be deterministic and tested.

### 13.3 Content evolution warning

Because saves store IDs referencing content definitions:

- renaming/removing content IDs must be handled via migration or alias mapping.
  Maintain an `IdAliasMap` for deprecated IDs.

------

## 14. Scaling Path: When to move beyond JSON

Move to SQLite/chunked persistence if:

- frequent autosaves cause frame spikes due to whole-file rewrite
- per-phase delta sets become large (many modified entities)
- you need partial updates (cell flush on unload)
- you want crash-safe transactional persistence

### 14.1 Hybrid approach (recommended at scale)

- SQLite stores:
  - meta
  - global save blobs (player/quests/gm)
  - per-cell delta index
- Chunk files store:
  - large cell delta blobs by cell id

This keeps streaming efficient and avoids huge monolithic files.

------

## 15. Invariants & Guardrails

1. Never store engine references in save data.
2. All persisted references are stable IDs.
3. Per-phase deltas must never apply outside their phase.
4. WorldObjectState is global and selective—do not overuse it.
5. Quest phase gating must be data-driven.
6. Save migrations are mandatory once public saves exist.