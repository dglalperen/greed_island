# Greed Island

Greed Island is a Unity 6 gameplay prototype focused on an original, rule-based aura ability system.

The project now uses a production-oriented `_Project` layout and a modular runtime architecture:

- Character locomotion (Character Controller, camera-relative movement)
- Aura resource + aura modes
- ScriptableObject-driven ability framework (conditions + effects)
- Combat test target and physics-reactive props
- HUD + debug state overlay

## Current Foundation (Implemented)

- Unity 6.3 (`6000.3.11f1`) + URP
- Input System action maps: `Player`, `UI`, `Debug`
- Scene set: `Bootstrap`, `PrototypeArena`, `AbilitySandbox`, `AnimationTest`
- Bootstrap flow: `Bootstrap` autoloads `PrototypeArena`
- Character features:
  - idle/move/sprint
  - jump + gravity
  - dash + cooldown
  - grounded checks + slope-aware projection
- Aura system:
  - pool (current/max)
  - regen with delay
  - spend/restore
  - modes (`Neutral`, `Concealment`, `Reinforcement`, `Expansion`, `Perception`)
- Ability framework:
  - `AbilityDefinition` + runtime `AbilityRunner`
  - activation validation (conditions)
  - aura cost + cooldown handling
  - effect execution pipeline
  - toggle/channel upkeep support
- Prototype abilities (shared framework):
  - `Aura Burst` (offense + force)
  - `Aura Guard` (defense toggle)
  - `Sense Pulse` (reveal utility)
- Combat layer:
  - `IDamageable` contract
  - `DamagePayload`, `DamageableActor`, `ShieldComponent`
  - `TargetDummyController`
- UI:
  - health bar
  - aura bar
  - cooldown rows (slots 1-3)
  - aura mode and ability status
  - debug panel (state, grounded, speed, aura, cooldowns)

## Canonical Folder Structure

```text
Assets/
  _Project/
    Art/
      Materials/
      Models/
      Animations/
      VFX/
      Audio/
    Prefabs/
      Character/
      Camera/
      Abilities/
      Combat/
      World/
      UI/
    Scenes/
      Bootstrap/
        Bootstrap.unity
      Prototype/
        PrototypeArena.unity
      Test/
        AbilitySandbox.unity
        AnimationTest.unity
    Scripts/
      Core/
      Input/
      Character/
      Camera/
      Combat/
      Stats/
      Aura/
      Abilities/
      AI/
      UI/
      Utilities/
      Tests/
    ScriptableObjects/
      Abilities/
      Aura/
      Stats/
      Tags/
    Settings/
    Gizmos/
```

## Notes About Runtime Setup

To keep the repository implementation-first while scenes/prefabs are still being authored, a runtime installer (`PrototypeArenaInstaller`) builds a playable arena setup when loading `PrototypeArena`, `AbilitySandbox`, or `AnimationTest`.

This includes:
- player object + required gameplay components
- camera follow rig
- target dummy
- rigidbody crates
- HUD + debug overlay

As authored prefabs/scenes are added, this installer can be retired or reduced.

## Package Requirements

`Packages/manifest.json` includes:
- `com.unity.inputsystem`
- `com.unity.render-pipelines.universal`
- `com.unity.ugui`
- `com.unity.cinemachine`

## Next Recommended Steps

1. Replace runtime-generated setup with authored prefabs and scene wiring.
2. Add animator controller + blend tree assets and connect triggers/parameters.
3. Convert prototype runtime ability definitions into persisted `ScriptableObject` assets.
4. Add EditMode/PlayMode tests for locomotion, aura, and ability execution.
5. Expand AI and lock-on behavior for combat iteration.
