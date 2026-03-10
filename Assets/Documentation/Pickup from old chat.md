What I understand is this:

Dragon’s Dogma: Dark Arisen combat is not “combo tree action combat” in the character-action sense. It is a **vocation-driven action RPG combat system** where the playable feel comes from four layers working together:

1. **Base weapon handling and core skills**
   Every vocation has weapon-specific fundamentals: light/heavy strings, mobility actions, blocks/deflects, jumps, controlled falls, etc. Core Skills are separate from the slotted weapon skills, are tied to the equipped weapon, and do not consume stamina.
2. **Slotted weapon skills / spell skills**
   The player equips a limited set of active skills on their weapon loadout, and those are the high-expression actions: shield skills, sword skills, bow skills, dagger skills, magick casts, counters, mobility techniques, enchants, traps, and so on. Skills are vocation/weapon dependent and are learned with Discipline Points as vocation rank increases.
3. **Stamina as the universal combat/mobility resource**
   Stamina is the common limiter on high-value actions: weapon skills, sprinting, climbing, and other exertive actions. That makes combat and traversal part of one continuous economy instead of separate systems. Core skills are the important exception: they generally do not consume stamina.
4. **Encounter expression through vocation identity, enemy interaction, and positioning**
   Vocation matters a lot: Fighter is grounded defense/control, Warrior is heavy commitment melee, Assassin gets mobility/counters/sniping, Mystic Knight mixes melee and magick support, etc. Enemies are not solved by a universal ruleset; the player uses vocation tools, positioning, knockdown, weak points, status, aerial access, and climbing to solve different targets.

So if I compress that into one sentence: **DDDA combat is a vocation- and weapon-defined action framework where a small set of equipped skills sits on top of strong weapon fundamentals, and stamina unifies mobility, offense, defense, and monster interaction.**

## What I think that means for your target architecture

Your current architecture is already moving in the right direction for that target.

### 1. Intent-first design is the right backbone

DDDA-style combat has many input meanings that are contextual:

- same face button can mean base attack, weapon skill, contextual action, traversal action
- same held state can mean aim/block/modifier/cast posture
- same movement input has different meaning depending on vocation/state/weapon

Your pipeline of:

- input snapshot
- resolve low-level intents
- arbitrate semantic conflicts
- synthesize traversal-action intents
- stamina gate
- pass approved intents into domain systems

is exactly the right shape for that. It separates **what the player tried to do** from **what the game decided that means**.

### 2. Combat mode / stance ownership is on the right path

DDDA is heavily modeful. Weapon set, stance, guard/aim/cast mode, and equipped vocation determine what action table is available. Your `CombatModeSystem` is currently simple, but architecturally it is the correct place for “which action space is currently active?” That becomes crucial once you stop thinking in terms of “attack button” and start thinking in terms of “action bank available under this weapon/vocation/mode.”

### 3. Stamina as an upstream arbiter is very aligned

That is one of the stronger decisions you’ve made. DDDA does not treat movement stamina and combat stamina as separate. Sprinting, climbing, active skills, and other exertive actions all live in one economy. Your new stamina system being upstream of traversal and action consumption is consistent with that style.

### 4. Traversal/action separation is correct, but only if they remain coordinated

DDDA’s mobility is deeply entangled with combat feel, but it is not the same thing as weapon action logic. Your split is good:

- traversal owns locomotion state
- action owns attack/skill/interact state
- stamina and intent arbitration sit above both

That is the right direction. The danger would be letting traversal become a second action system, or letting action system start owning locomotion semantics.

## Where your current architecture is still short of DDDA

This is the important part.

### 1. Your `ActionSystem` is still basically an animation trigger router

Right now it does not yet own the things that would actually make DDDA-like combat work:

- action definitions
- action lifecycle
- startup/active/recovery
- lockouts / commitment
- buffering
- cancel windows
- stamina ownership per concrete action
- weapon/vocation loadout routing
- hit rules / defensive rules
- cast/channel timing
- charged/held actions
- counter/parry windows

Until that exists, your architecture has the shell of the right system, but not the combat substance.

### 2. You do not yet have a real “equipped action table”

DDDA is built around vocation + weapon + equipped skills. The player is not invoking generic `UseSkill(slot)` forever; they are invoking **the currently equipped skill in that slot for the current loadout**, and each of those has its own behavior, stamina cost, timing, targeting, and permission rules.

So eventually your `UseSkillIntent(bank, slot)` needs to resolve into something like:

- concrete action definition
- with execution policy
- with resource cost
- with animation/event data
- with cancel/buffer metadata

That is where DDDA actually starts to emerge.

### 3. Core skills vs weapon skills is not modeled yet

This matters a lot.

DDDA distinguishes between:

- basic/core weapon abilities
- slotted special skills
- augments/passives

Your current architecture has a hint of that split, but not the real model yet. At the moment, some things are still just raw intents or direct animation triggers. Eventually you need three different layers:

- **core moveset**
- **equipped active skills**
- **passive modifiers/augments**

That distinction is foundational to DDDA. 

### 4. Enemy interaction layer is mostly absent

DDDA combat is not just about player moves; it is heavily about what those moves do to enemies:

- stagger
- knockdown
- climbing
- weak-point exploitation
- defensive reactions
- target-specific solutions
- party/pawn interaction

Your current code is still almost entirely player-local. That is fine for the stage you are at, but it means you are not near the “real” DDDA feel yet.

### 5. Vocation identity is not represented yet

This is probably the biggest strategic gap.

DDDA combat is not one player system with a few different attacks. It is a family of strongly differentiated move systems constrained by vocation and weapon type. The architecture needs an eventual answer for:

- who defines the allowed actions for this vocation?
- who defines legal weapon pairings?
- who provides core skill set for current equipment?
- who provides equipped skill banks?
- who applies vocation augments/passives?

Until that layer exists, you can only build generic player-combat scaffolding.

## The architecture direction I think you’re heading toward

If I extrapolate cleanly from what you have, I think the eventual system wants to look like this:

### Input / intent layer

Purely answers:

- what did the player request?

### Arbitration / synthesis / resources

Answers:

- what does that request mean in current context?
- what derived actions emerge from context?
- can the character afford it?

### Combat mode / loadout layer

Answers:

- given current vocation, weapon set, and mode, what action table is active?
- what does slot 1/2/3 actually map to right now?
- what core skills are present?

### Action system

Answers:

- can this specific action start now?
- if so, enter action state and own its lifecycle
- emit animation/motor/gameplay windows
- honor buffering / cancels / commitment
- consume stamina / cooldown / ammo / cast time as defined by the action

### Traversal system

Answers:

- locomotion state and transitions
- traversal-side semantics like slide/leap/glide/drop/climb/wall-run later

### Effects / combat resolution layer

Answers:

- what happened to the target?
- stagger, knockdown, damage, status, hit reactions, mount/climb consequences

That overall shape is compatible with DDDA-style combat.

## The one thing I’d watch carefully

Do not let `ActionSystem` become a giant procedural `if intent then trigger animation` blob with a few stamina checks bolted on. If you do that, you will hit a wall the moment you try to model:

- charged skills
- parries
- counters
- channeling spells
- jump-cancellable attacks
- climbing-only actions
- vocation-specific alternate move sets
- equipment-driven skill swaps

DDDA needs **action definitions**, not just action handling.

## So, to summarize my understanding

I think your end goal is something like this:

- a player framework where **vocation/loadout determines action vocabulary**
- core moves and special skills are separate concepts
- stamina is shared across traversal and combat
- input intent is contextual and must be arbitrated before execution
- actions are not immediate one-frame triggers; they are stateful executions with rules
- traversal and combat remain separate domains but are coordinated through intent and resource policy
- later, enemies and climbing/monster interaction become first-class combat problems

If that is your target, then your current architecture is pointed in the right direction, but the next real leap is not “more traversal.” It is **turning ActionSystem into a concrete action-definition and action-execution framework**.

Yes.

What you’re building on the combat side is **not** a simple “attack handler.” It is a layered **action framework** where combat is produced by resolving player intent into a concrete action, then letting that action execute over time under clear rules.

Below is the combat/action model as I currently understand it.

# Combat System Goal

The target is a combat system where:

- basic weapon actions and slotted skills are distinct concepts
- the current loadout determines what actions exist
- stamina is shared across combat and traversal
- actions are stateful and time-based
- the same input can mean different things depending on mode, equipment, and state
- actions can be buffered, committed, cancelled, or denied based on rules
- the player is not just “pressing a button,” but invoking a current action vocabulary

So the combat system is really made of **five layers**:

1. **Combat Mode**
2. **Loadout / Action Mapping**
3. **Action Resolution**
4. **Action Runtime**
5. **Combat Effects / Outcomes**

------

# 1) Combat Mode

## Purpose

Combat Mode determines **what action space the player is currently in**.

This is not the same thing as executing an action. It is the system that answers:

- am I in neutral traversal-ready mode?
- am I in armed combat stance?
- am I aiming, blocking, casting, charging, or otherwise in a modifier mode?
- what category of actions is even available right now?

## Why it exists

The same input cannot always mean the same thing.

For example, a face button might mean:

- light attack in neutral melee stance
- a slotted skill while a modifier is held
- a contextual interaction in neutral state
- a traversal action in a traversal-specific context

Combat Mode narrows that action space before action execution even begins.

## What it should own

Combat Mode should own:

- stance transitions
- modifier modes
- upper-body mode or combat posture
- whether the player is in a combat-available or traversal-dominant context
- later: weapon-set-dependent mode restrictions

## What it should not own

Combat Mode should **not** own:

- actual attack timing
- stamina spending for each action
- buffering/cancel rules
- hit logic

It only defines **the currently active combat vocabulary**.

------

# 2) Loadout / Action Mapping

## Purpose

This layer determines **what concrete actions are bound to the current player setup**.

That means the system must answer:

- what is “light attack” right now?
- what skill is assigned to slot 1 right now?
- what are the player’s core actions for the current weapon/loadout?
- what action families are legal for the current equipment/class/role?

## Key distinction

You need to separate three things:

### Core Actions

These are the fundamental built-in actions of the current combat style.

Examples conceptually:

- base light attack chain
- heavy attack
- guard
- aim shot
- dodge
- charged basic attack
- shield bash
- basic cast channel
- jump strike
- other weapon fundamentals

These are not “equipped skill slots.” They are part of the baseline moveset.

### Equipped Active Skills

These are slotted, user-selected combat abilities.

Examples conceptually:

- weapon technique 1
- weapon technique 2
- mobility/counter skill
- special ranged skill
- casted spell
- support ability

These are loadout-dependent and should resolve from slot requests.

### Passive / Always-On Modifiers

These do not create actions directly, but modify them.

Examples:

- reduced stamina use
- altered cancel windows
- stronger stagger
- faster charge
- improved guard response

These belong in the combat framework eventually, but not as executable actions.

## Why this matters

If you do not make this distinction early, the action system collapses into “everything is a skill” or “everything is just an attack button,” and both are wrong.

------

# 3) Action Resolution

## Purpose

Action Resolution takes an approved intent and turns it into a **concrete action definition**.

This is where the system answers:

> Given the player’s current mode, loadout, state, and approved intent, what exact action is being requested?

## Inputs to action resolution

The resolver should inspect:

- approved intent list
- current combat mode
- current traversal mode
- current equipment/loadout
- current action runtime state
- possibly resource state
- later: target/context/environment info

## Output of action resolution

It should produce a **concrete action request** or **action definition**, not just a bool.

For example:

- `LightAttackIntent` -> `SwordLightAttack_Chain1`
- `UseSkillIntent(Primary, 1)` -> `EquippedPrimarySkillSlot1`
- `HeavyAttackIntent` -> `ChargedHeavyStart`
- `ContextInteractIntent` -> `ContextGrabAction`
- same intent in another mode might resolve to something totally different

## Why this layer is essential

Without this, `ActionSystem` becomes a giant hardcoded switch on intents, and that does not scale.

The resolver is the translation boundary between:

- abstract player request
- concrete combat action

------

# 4) Action Runtime

This is the real heart of the combat system.

## Purpose

Once an action has been resolved, the runtime owns that action until it completes, is cancelled, or is replaced under valid rules.

This is how actions stop being one-frame triggers and become actual gameplay execution.

## What an action is

A concrete action should have a definition with properties like:

- action id
- action category
- source type (`Core`, `Skill`, `Context`, etc.)
- startup duration
- active duration
- recovery duration
- stamina cost
- allowed cancel rules
- allowed buffering rules
- movement policy during action
- animation trigger / animation state info
- gameplay tags like `Melee`, `Ranged`, `Counter`, `Spell`, `Guard`, `Charge`, `Finisher`

## Runtime phases

At minimum, each action should progress through:

### Startup

The action has begun, but has not yet “gone live.”

Use cases:

- wind-up
- cast start
- draw-back
- anticipation
- startup commitment

### Active

The action is currently live.

Use cases:

- hit window
- projectile release
- guard reflection window
- spell effect window
- mobility burst
- interaction contact window

### Recovery

The action has completed its main effect, but the player is still committed.

Use cases:

- follow-through
- cooldown-like local lock
- landing delay
- cast end
- re-shouldering weapon

## Runtime state

The player model or action runtime state should track at least:

- current action id
- current phase
- phase elapsed time
- whether an action is active
- buffered next action request, if any

That is enough for the first skeleton.

## What the runtime should decide

The runtime owns answers to questions like:

- can a new action start right now?
- if not, can it be buffered?
- is the current action cancellable?
- does this new request replace the current one?
- has the current phase ended?
- should the next phase begin?
- should outputs fire this frame?

## Why this is the core of the whole design

Once this exists, you can support:

- committed attacks
- buffered chains
- held/charged actions
- cast/channel actions
- counters/parries
- skill execution windows
- cancel trees
- weapon-specific action flow

Without it, combat stays superficial.

------

# 5) Combat Effects / Outcomes

## Purpose

The action runtime says **what the player is doing**.
The combat effect layer says **what that action did**.

Eventually this layer should handle:

- hit confirms
- damage windows
- stagger
- knockdown
- defensive interactions
- projectile spawns
- status effects
- grabs
- mounted/climbing interactions
- enemy-specific reactions

This layer is not the first thing to build, but it must exist later.

Right now you can keep it as future-facing output hooks.

------

# Where Stamina Fits Into Combat

## Current role

Stamina is already correctly placed as an upstream resource authority.

That means it can:

- drain continuously for traversal states
- deny costly actions before execution
- consume cost on approved actions

## Long-term combat role

Once Action Resolution exists, stamina should no longer need to know every combat action by name.

Instead:

- traversal costs can remain explicit in stamina/traversal policy
- combat action costs should come from the **resolved action definition**

So the eventual flow should look like:

1. resolve intent into concrete action
2. inspect that action’s cost/policy
3. stamina system approves or denies it
4. runtime starts it if approved

That is cleaner than baking every skill’s stamina logic into the stamina system itself.

------

# Buffering

## Purpose

Buffering allows the player to queue an action slightly before the current action completes.

This is critical for responsive combat.

## Minimal first version

Support just one buffered action slot.

That means:

- if player presses a valid action during a locked phase
- and buffering is allowed
- store that action request
- when current action reaches a legal handoff point, start buffered action

## Why it matters

Without buffering, combat feels unresponsive.
With too much buffering, combat feels sticky.

So it needs to exist early, even in a simple form.

------

# Cancels

## Purpose

Cancels define when one action may interrupt another.

Examples conceptually:

- recovery cancel into dodge
- active cancel into follow-up combo step
- charge cancel into release
- block cancel into counter
- jump cancel from certain actions

## Important point

Cancels should not be random hardcoded exceptions inside `ActionSystem`.

They should be part of action definition and action policy.

For example:

- this action can cancel into `Dodge`
- this action can cancel into `Skill`
- this action can cancel only during startup
- this action cannot cancel at all

That gives you a structured combat grammar.

------

# Commitment

## Purpose

Commitment is what stops combat from becoming spammy input noise.

An action can begin and then own the player until:

- the phase ends
- a legal cancel occurs
- the action resolves into another valid state

Commitment is one of the major reasons the runtime has to exist.

------

# How Traversal and Combat Interact

Traversal and combat should stay separate, but they must coordinate.

## Traversal owns

- locomotion states
- grounded/airborne/sliding/climbing/etc.
- movement transitions
- traversal-specific semantics

## Combat owns

- action execution
- weapon/skill behavior
- combat timing
- buffering/cancels
- combat-specific permissions

## Shared coordination points

They meet through:

- intent pipeline
- stamina/resource policy
- player model
- outputs
- action permission rules

That allows combat to care about traversal context without traversal becoming combat code.

Examples:

- some actions only legal while grounded
- some actions only legal while airborne
- some actions modify movement during execution
- some traversal states forbid combat actions
- some combat actions trigger traversal transitions

That coordination should happen through defined interfaces, not direct ownership leakage.

------

# Generic Execution Flow for Combat

A typical combat tick should eventually look like this:

## Step 1: approved intents arrive

The player has already gone through:

- input resolution
- intent arbitration
- traversal-action synthesis
- stamina/resource gating

So combat receives **approved requests**, not raw input noise.

## Step 2: combat mode is updated

The system updates stance/modifier mode so action resolution knows what action vocabulary is active.

## Step 3: action resolver inspects approved intents

The resolver looks for the highest-priority legal action request and maps it to a concrete action definition.

## Step 4: runtime decides whether it can start

If no action is currently running:

- start the new action

If an action is running:

- reject
- buffer
- or cancel/replace
  depending on policy

## Step 5: runtime ticks current action

Advance timers and phase transitions.

## Step 6: outputs are emitted

Examples:

- animation trigger on action start
- gameplay active window during active phase
- end-of-action reset during recovery exit

## Step 7: action completes or chains

If action ends:

- clear runtime state
- possibly start buffered action
- return to neutral action availability

------

# The First Skeleton to Build

The first combat/action skeleton should be deliberately small.

## Minimum viable implementation

Support:

- one concrete light attack action
- one concrete heavy attack action
- one slotted skill action
- startup / active / recovery
- one buffered next action
- simple no-cancel policy at first
- action start animation trigger
- action completion state clear

## What this proves

If that works, then you have proven:

- intent can resolve into action definitions
- actions can execute over time
- runtime ownership works
- buffering works
- the architecture is viable for expansion

That is the real first milestone.

------

# Suggested Internal Structure

A clean structure would be:

## `Action/Definitions`

Contains data describing actions.

- `PlayerActionDefinition`
- `PlayerActionCategory`
- `PlayerActionTags`
- `PlayerActionTiming`
- `PlayerActionCancelPolicy`

## `Action/Resolution`

Maps approved intents and current context to concrete actions.

- `PlayerActionResolver`
- maybe `PlayerActionLoadout` later

## `Action/Runtime`

Owns action execution state.

- `PlayerActionRuntimeState`
- `PlayerActionPhase`
- `ActionSystem`

## `Action/Loadout`

Later, this should own equipped skill/core-action mapping.

- core moveset definition
- equipped skills by slot
- future class/weapon/vocation mapping

------

# Short Version

The combat/action system you are building should work like this:

- **Combat Mode** decides what action vocabulary is currently available.
- **Loadout Mapping** decides what concrete actions exist for the current setup.
- **Action Resolution** turns approved intents into specific actions.
- **Action Runtime** owns those actions over time through startup/active/recovery, buffering, and cancel rules.
- **Combat Effects** later determine what those actions do to the world and enemies.
- **Stamina** remains a shared upstream authority across traversal and combat.

That is the correct skeleton for the kind of system you are aiming at.

If you want, next I can turn this into a concrete implementation plan with exact first-pass files for `Action/Definitions`, `Action/Runtime`, and `Action/Resolution`.