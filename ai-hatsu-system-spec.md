# AI-Driven Aura Ability System Spec
Version: 0.1
Project: Greed Island
Engine: Unity
Language: C#
Focus: Third-person sandbox with aura-based progression and AI-assisted custom ability generation

---

# 1. Purpose

This document defines the gameplay and technical design for an aura-based character progression and custom ability generation system.

The goal is to create a system where:

1. Each player begins with an **affinity type**, but no personal signature ability.
2. The player learns and trains foundational aura techniques first.
3. After sufficient progression, the player may describe a desired signature ability in natural language.
4. An AI interprets that request.
5. The game validates the request against world rules, progression level, affinity compatibility, and gameplay constraints.
6. If valid, the game generates a structured ability definition that can be executed by the gameplay systems.

The AI must **not** directly invent arbitrary gameplay logic.
The AI acts as an **interpreter and structured designer assistant**.
The game systems remain the source of truth.

---

# 2. High-Level Design Philosophy

## 2.1 Core Principles

- Players should **earn** custom ability creation through gameplay progression.
- Fundamental aura control comes before signature ability creation.
- Signature abilities must feel:
  - personal
  - rule-based
  - costly
  - restricted
  - mechanically meaningful
- The system should prioritize:
  - creativity
  - trade-offs
  - clarity
  - systemic validation

## 2.2 Rule of Authority

The final authority is:

1. Game validation rules
2. Supported gameplay effect library
3. Progression system
4. AI interpretation layer

The AI may suggest.
The game decides.

---

# 3. Player Progression Flow

## 3.1 Phase 1 - Character Initialization

The player begins by answering a set of questions.

### Goals
- Determine personality tendencies
- Determine preferred problem-solving style
- Determine risk profile
- Determine combat mindset
- Determine affinity compatibility

### Outputs
- `PrimaryAffinity`
- `SecondaryAffinityLeanings`
- `TemperamentProfile`
- `InitialAuraStats`

### Notes
At this stage, the player does **not** receive a signature ability.

---

## 3.2 Phase 2 - Foundational Aura Training

The player must train foundational aura techniques before unlocking custom signature ability creation.

### Foundational Techniques
- Aura Stability
- Aura Suppression
- Aura Expansion
- Aura Reinforcement
- Aura Focus
- Optional advanced techniques later

### Gameplay Purpose
- Teach the player the world rules
- Teach the player aura resource control
- Record behavioral playstyle data
- Gate signature ability creation behind mastery

### Player Metrics to Record
- preferred combat distance
- aggression level
- mobility preference
- aura efficiency
- defensive behavior
- reaction speed
- preferred timing windows
- tendency toward traps/control/direct force

These metrics should influence later interpretation and balancing.

---

## 3.3 Phase 3 - Signature Ability Unlock

When the player reaches required mastery thresholds, they unlock a special sequence where they may propose a signature ability.

### Unlock Requirements
Example requirements:
- minimum aura mastery level
- minimum foundational technique progression
- minimum combat test completion
- optional narrative milestone

### Player Inputs During Proposal
The player should be able to describe:
- what the ability does
- how it activates
- who or what it targets
- what restrictions it has
- what costs or sacrifices it should include
- whether it is offensive, defensive, supportive, deceptive, control-oriented, or utility-oriented
- any conditions required for stronger output

---

# 4. System Architecture Overview

The signature ability pipeline must follow this order:

1. Player natural language request
2. AI interpretation
3. Structured ability schema output
4. Validation layer
5. Balancing layer
6. Ability compilation into game data
7. Runtime execution through the ability framework

---

# 5. Supported Runtime Domains

## 5.1 Affinity Domain

Each player has an affinity profile.

### Required Fields
- `PrimaryAffinity`
- `SecondaryLeanings`
- `AffinityEfficiencyModifiers`
- `AffinityRestrictions`

### Example Internal Affinity Names
Use original terminology, not copyrighted names.

Example:
- Reinforcement
- Projection
- Control
- Manifestation
- Alteration
- Unique

---

## 5.2 Aura Domain

Aura is the main power resource.

### Aura Responsibilities
- current value
- max value
- regeneration
- regeneration delay
- upkeep drain
- burst spending
- state modifiers
- temporary buffs/debuffs

### Aura States
Example internal states:
- Neutral
- Concealment
- Reinforcement
- Expansion
- Perception

Each state may modify:
- offense
- defense
- detectability
- sensing
- upkeep cost
- regen behavior

---

## 5.3 Ability Domain

All abilities must be represented in a structured way.

### Ability Definition Core
Every ability must define:
- identity
- affinity bias
- activation method
- costs
- conditions
- targeting
- restrictions
- effects
- scaling
- failure conditions

---

# 6. AI Role

## 6.1 AI Responsibilities

The AI is responsible for:
- interpreting player intent
- extracting structured rules from free text
- proposing balanced restrictions
- converting vague fantasy into supported gameplay verbs
- identifying contradictions
- identifying missing activation conditions
- identifying unsupported requests

## 6.2 AI Must Not
The AI must not:
- invent unsupported engine features
- bypass game validation
- create impossible effects
- ignore affinity compatibility
- ignore cost/restriction balance
- directly write raw runtime behavior without schema compliance

---

# 7. Supported Ability Construction Model

All generated abilities must be built from supported categories.

## 7.1 Activation Types
- Instant
- Casted
- Charged
- Toggle
- Channeled
- Triggered
- Passive
- ConditionalPassive

## 7.2 Targeting Types
- Self
- SingleTarget
- ForwardRay
- Cone
- Sphere
- RadiusPulse
- AreaPlacement
- MarkedTarget
- Line
- MultiTargetLimited

## 7.3 Cost Types
- FlatAuraCost
- PercentageAuraCost
- AuraUpkeep
- SelfSlow
- SelfDamage
- CooldownExtension
- TemporarySenseLoss
- RestrictedMovement
- LimitedUsesPerEncounter

## 7.4 Restriction Types
- RequiresPhysicalContact
- RequiresLineOfSight
- RequiresVerbalDeclaration
- OnlyWorksOnMarkedTargets
- LimitedTargetCount
- BreaksIfUserMoves
- BreaksIfUserTakesDamage
- WorksOnlyInRange
- WorksOnlyAfterConditionMet
- RequiresPreparation
- RequiresChargeTime
- CannotBeUsedAgainUntilConditionReset

## 7.5 Effect Types
- Damage
- Knockback
- Pull
- Push
- Mark
- Reveal
- Shield
- BuffSelf
- DebuffTarget
- Root
- Slow
- Dash
- TeleportShortRange
- SpawnProjectile
- SpawnConstruct
- AuraPulse
- CounterState
- TriggeredPunishment
- Heal
- DrainAura
- RedirectForce

Only supported effect types may be used.

---

# 8. Signature Ability Schema

The AI must output the requested ability in a structured schema.

## 8.1 Canonical Schema

```json
{
  "abilityName": "",
  "fantasySummary": "",
  "archetypeTags": [],
  "affinityBias": [],
  "activationType": "",
  "targetingType": "",
  "range": 0,
  "radius": 0,
  "maxTargets": 0,
  "activationRequirements": [],
  "restrictions": [],
  "costs": [],
  "effects": [],
  "scalingModel": {},
  "riskLevel": "",
  "complexityScore": 0,
  "suggestedRarity": "",
  "validationNotes": [],
  "unsupportedFantasyParts": []
}
8.2 Example Output
{
  "abilityName": "Grave Thread",
  "fantasySummary": "Marks up to two touched targets and forcefully pulls them back when they violate a declared condition.",
  "archetypeTags": ["control", "trap", "punishment"],
  "affinityBias": ["Control", "Projection"],
  "activationType": "Triggered",
  "targetingType": "MarkedTarget",
  "range": 25,
  "radius": 0,
  "maxTargets": 2,
  "activationRequirements": [
    "User must physically touch target once",
    "User must verbally declare trigger condition"
  ],
  "restrictions": [
    "Maximum two targets",
    "Effect ends if user is stunned",
    "No aura regeneration while active"
  ],
  "costs": [
    {
      "type": "FlatAuraCost",
      "value": 20
    },
    {
      "type": "AuraUpkeep",
      "value": 8
    }
  ],
  "effects": [
    {
      "type": "Mark",
      "value": 1
    },
    {
      "type": "Pull",
      "value": 12
    },
    {
      "type": "TriggeredPunishment",
      "value": 1
    }
  ],
  "scalingModel": {
    "touchRequired": true,
    "triggerStrengthIncreasesWithPreparation": true
  },
  "riskLevel": "MediumHigh",
  "complexityScore": 7,
  "suggestedRarity": "Rare",
  "validationNotes": [
    "Strong control effect justified by contact requirement and upkeep drain"
  ],
  "unsupportedFantasyParts": []
}
9. Validation Layer

The validation layer is mandatory.

9.1 Validation Goals

The validator must check:

Is the requested fantasy expressible using supported effect types?

Is the ability compatible with the player's affinity?

Are the restrictions strong enough for the requested output?

Is the power budget within the player's progression tier?

Are all required runtime values present?

Is targeting valid?

Are there contradictory rules?

Is the ability too vague?

Is the requested complexity too high for the player's current unlock level?

9.2 Validation Outcomes

Possible outcomes:

Accepted

AcceptedWithSimplification

RejectedInsufficientRestrictions

RejectedUnsupportedMechanics

RejectedAffinityMismatch

RejectedProgressionLocked

RejectedAmbiguousDesign

9.3 Example Validation Errors

“This requested ability has no clear activation condition.”

“The requested area and power are too high for the current restriction level.”

“This effect exceeds your current aura mastery tier.”

“Your current affinity is poorly suited for this structure.”

“The requested effect cannot be represented by the supported gameplay systems.”

10. Balancing Model
10.1 Power Budget

Each generated ability must fit within a power budget.

Inputs to Budget

player progression tier

affinity efficiency

number of effects

range

radius

target count

persistence duration

control strength

damage output

defensive value

restrictions

costs

activation difficulty

10.2 Restriction Credit

Restrictions increase allowed power.

Examples of strong restrictions:

physical touch requirement

long charge time

verbal declaration

user vulnerability during cast

effect ends if condition breaks

severe upkeep drain

inability to use other techniques simultaneously

The stronger the restriction, the more effect budget may be allowed.

11. Progression Tiers

Generated abilities must scale by progression tier.

Example Tiers

Tier 0: No signature ability access

Tier 1: Basic single-effect signature

Tier 2: Single-effect + one condition/restriction

Tier 3: Multi-effect controlled ability

Tier 4: Advanced ability with trigger logic

Tier 5: High-complexity ability with layered restrictions

The validator must reject abilities above the current player tier.

12. Runtime Compilation Rules

Once validated, the ability must be converted into runtime data.

Required Runtime Output

AbilityDefinition

AbilityCondition[]

AbilityCost[]

AbilityEffect[]

TargetingProfile

CooldownConfig

AuraInteractionConfig

PresentationMetadata

Runtime Rule

The runtime should execute only compiled structured ability data.
It must never execute raw natural language.

13. World Reactivity Requirements

The world should react to the generated ability in meaningful ways.

Systems That Should Be Able to Read Ability Metadata

combat systems

AI threat evaluation

UI descriptions

progression logs

NPC dialogue hooks

tutorial hints

balancing telemetry

Example Metadata Tags

control

stealth

burst

punishment

counter

mobility

ranged

defensive

risky

high-upkeep

14. AI Prompting Rules

When the AI receives a player request, it should follow this process:

Summarize the player's desired fantasy

Identify likely affinity fit

Extract:

activation

target

restrictions

costs

effects

Detect unsupported or vague elements

Convert fantasy into supported gameplay verbs

Produce canonical schema

Add validation notes

Mark uncertain parts explicitly

The AI must prefer:

clarity over hype

structure over flavor text

supported mechanics over impossible fantasy

15. Example AI Input Format
Player Prompt Example

“I want an ability where I mark someone by touching them, and if they move away from a place I chose, they get dragged back. It should be hard to use and I only want to use it on a few people.”

AI Interpretation Goals

detect contact requirement

detect mark state

detect place-based trigger

detect pull effect

detect desire for built-in restriction

translate into supported schema

16. First Implementation Milestone

The first implementation should support only a limited subset.

Milestone 1 Supported Features

one affinity profile per player

foundational aura progression

one signature ability unlock sequence

AI interpretation into schema

validator

basic compiler

support for these effects only:

Damage

Knockback

Pull

Mark

Reveal

Shield

support for these restrictions only:

physical contact

line of sight

max targets

aura upkeep

charge time

self slow

Keep the first version intentionally small.

17. Non-Goals for First Version

Do not implement these in version 1:

unrestricted freeform powers

permanent reality-warping effects

time manipulation

full PvP balancing

procedural animation generation

dynamic code generation from prompts

multiplayer authority handling

open-ended simulation of every fantasy

18. Technical Deliverables Expected From an AI Implementer

An AI implementing this system should produce:

Folder structure

Runtime architecture

C# class list

ScriptableObject schema definitions

Validator design

Compiler design

Example generated ability assets

First supported effect library

Example prompt-to-schema pipeline

Unity integration plan

19. Success Criteria

This system is successful if:

Players do not get a custom signature ability immediately

Foundational aura progression matters

The AI can interpret player requests into structured format

The validator can reject or simplify bad requests

The runtime can execute compiled generated abilities

Generated abilities feel personal but still rule-bound

The world can react meaningfully to ability metadata

20. Future Extensions

Planned future extensions:

more effect types

more restriction types

affinity-specific biasing

AI-assisted refinement conversation

ability evolution over time

narrative consequences for chosen restrictions

AI mentor / examiner sequence for ability approval

NPC recognition of unusual power styles