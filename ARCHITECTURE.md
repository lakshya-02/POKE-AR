# AR Pokemon Battle - System Architecture

## 📊 Component Flow Diagram

```
┌─────────────────────────────────────────────────────────────┐
│                     AR SESSION ORIGIN                        │
│  ┌──────────────────────────────────────────────────────┐   │
│  │         AR Tracked Image Manager                      │   │
│  │  - Detects Pokemon card images                       │   │
│  │  - Reference Image Library (Charizard, Pikachu)      │   │
│  └──────────────────────────────────────────────────────┘   │
│                            ↓                                  │
│  ┌──────────────────────────────────────────────────────┐   │
│  │            ImageTracker.cs                            │   │
│  │  - Spawns Pokemon prefabs at tracked positions       │   │
│  │  - Manages tracking state (visible/hidden)           │   │
│  └──────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
                            ↓
                     SPAWNS POKEMON
                            ↓
        ┌──────────────────┴──────────────────┐
        ↓                                      ↓
┌────────────────┐                    ┌────────────────┐
│   CHARIZARD    │                    │    PIKACHU     │
│   (Prefab)     │                    │    (Prefab)    │
├────────────────┤                    ├────────────────┤
│ PokemonController                   │ PokemonController
│ - HP: 200      │                    │ - HP: 150      │
│ - Damage: 60   │                    │ - Damage: 40   │
│ - Move: Fire   │                    │ - Move: Elec   │
│ - Animator     │                    │ - Animator     │
│ - Collider     │                    │ - Collider     │
└────────────────┘                    └────────────────┘
        ↓                                      ↓
        └──────────────────┬──────────────────┘
                           ↓
                 BOTH DETECTED
                           ↓
        ┌──────────────────────────────────────┐
        │       BattleManager.cs               │
        │  - Detects both Pokemon spawned      │
        │  - Initiates battle mode             │
        │  - Manages turn system               │
        │  - Spawns move projectiles           │
        │  - Tracks HP and winner              │
        └──────────────────────────────────────┘
                ↓                      ↓
        ┌───────────┐          ┌──────────────┐
        │   UI      │          │ MovesManager │
        │ Buttons   │          │ - Move data  │
        │ HP Text   │          │ - Damage     │
        │ Status    │          │ - Names      │
        └───────────┘          └──────────────┘
                ↓
        PLAYER TAPS ATTACK
                ↓
        ┌──────────────────────────────────────┐
        │  Projectile Spawned                  │
        │  - MoveProjectile.cs                 │
        │  - Physics (Rigidbody)               │
        │  - Collision detection               │
        └──────────────────────────────────────┘
                ↓
        FLIES TOWARD TARGET
                ↓
        ┌──────────────────────────────────────┐
        │  COLLISION DETECTED                  │
        │  - OnTriggerEnter()                  │
        │  - Apply damage to target            │
        │  - Play hit animation                │
        │  - Update HP                         │
        └──────────────────────────────────────┘
                ↓
        ┌──────────────────────────────────────┐
        │  CHECK HP                            │
        │  - If HP > 0: Continue battle        │
        │  - If HP = 0: Pokemon faints         │
        └──────────────────────────────────────┘
                ↓
        ┌──────────────────────────────────────┐
        │  BATTLE END                          │
        │  - Winner declared                   │
        │  - Battle inactive                   │
        │  - Option to reset                   │
        └──────────────────────────────────────┘
```

## 🎯 Throwing Mechanism Detail

```
┌─────────────────────────────────────────────────────────┐
│           METHOD 1: Button + Auto-Target                │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  [Player Taps Button]                                   │
│          ↓                                               │
│  BattleManager.OnCharizardAttack()                      │
│          ↓                                               │
│  ThrowMove(charizard, pikachu, movePrefab)             │
│          ↓                                               │
│  1. Spawn prefab at attacker.projectileSpawnPoint      │
│  2. Setup MoveProjectile component                      │
│  3. Set target = opponent                               │
│  4. projectile.LaunchAtTarget()                         │
│          ↓                                               │
│  Projectile auto-homes to target.hitPoint              │
│                                                          │
└─────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────┐
│           METHOD 2: Swipe + Manual Throw                │
├─────────────────────────────────────────────────────────┤
│                                                          │
│  [Player Touches Pokemon]                               │
│          ↓                                               │
│  SwipeProjectileLauncher detects touch                  │
│          ↓                                               │
│  [Player Swipes]                                        │
│          ↓                                               │
│  1. Calculate swipe vector (direction + speed)         │
│  2. Determine which Pokemon touched                     │
│  3. Spawn move prefab                                   │
│  4. projectile.Launch(direction, speed)                 │
│          ↓                                               │
│  Projectile flies in swipe direction                    │
│  (visual feedback shows swipe path)                     │
│                                                          │
└─────────────────────────────────────────────────────────┘
```

## 🔄 Animation State Flow

```
        POKEMON SPAWNED
              ↓
        [IDLE ANIMATION]
        (continuous loop)
              ↓
    ┌─────────┴─────────┐
    ↓                    ↓
ATTACK BUTTON        RECEIVES HIT
    ↓                    ↓
[ATTACK ANIM]      [HIT ANIMATION]
    ↓                    ↓
    └─────────┬──────────┘
              ↓
        CHECK HP
              ↓
    ┌─────────┴─────────┐
    ↓                    ↓
  HP > 0              HP = 0
    ↓                    ↓
[IDLE ANIM]      [FAINT ANIMATION]
(loop back)        (battle ends)
```

## 🎮 Input Handling Options

```
┌──────────────────────────────────────────────────────┐
│  INPUT OPTION 1: UI Buttons                          │
├──────────────────────────────────────────────────────┤
│  Pros:                                               │
│  ✓ Simple and reliable                              │
│  ✓ Clear feedback                                   │
│  ✓ Works on all devices                             │
│                                                       │
│  Cons:                                               │
│  ✗ Less interactive                                 │
│  ✗ Automatic targeting (no skill)                   │
├──────────────────────────────────────────────────────┤
│  Use when: Targeting simplicity over interaction    │
└──────────────────────────────────────────────────────┘

┌──────────────────────────────────────────────────────┐
│  INPUT OPTION 2: Swipe Gestures                      │
├──────────────────────────────────────────────────────┤
│  Pros:                                               │
│  ✓ Very interactive                                 │
│  ✓ Skill-based aiming                               │
│  ✓ More engaging gameplay                           │
│  ✓ Visual swipe feedback                            │
│                                                       │
│  Cons:                                               │
│  ✗ Requires practice                                │
│  ✗ Can miss target                                  │
│  ✗ More complex to implement                        │
├──────────────────────────────────────────────────────┤
│  Use when: Prioritizing engagement and skill        │
└──────────────────────────────────────────────────────┘
```

## 📦 Prefab Structure

```
CHARIZARD PREFAB
├── Charizard (Root GameObject)
│   ├── PokemonController component
│   ├── BoxCollider / MeshCollider
│   └── Child: 3D Model
│       └── Animator component
├── ProjectileSpawn (empty, offset forward/up)
└── HitPoint (empty, center of model)

PIKACHU PREFAB
├── Pikachu (Root GameObject)
│   ├── PokemonController component
│   ├── BoxCollider / MeshCollider
│   └── Child: 3D Model
│       └── Animator component
├── ProjectileSpawn (empty, offset forward/up)
└── HitPoint (empty, center of model)

MOVE PREFAB (Flamethrower)
├── Flamethrower (Root GameObject)
│   ├── MoveProjectile component
│   ├── Rigidbody (no gravity)
│   ├── SphereCollider (trigger)
│   ├── MeshRenderer (visual)
│   ├── TrailRenderer (effect)
│   ├── Light (glow)
│   └── Optional: ParticleSystem

MOVE PREFAB (Thunderbolt)
├── Thunderbolt (Root GameObject)
│   ├── MoveProjectile component
│   ├── Rigidbody (no gravity)
│   ├── SphereCollider (trigger)
│   ├── MeshRenderer (visual)
│   ├── TrailRenderer (effect)
│   ├── Light (glow)
│   └── Optional: ParticleSystem
```

## 🎪 Scene Hierarchy

```
AR Pokemon Battle Scene
├── AR Session
├── AR Session Origin
│   ├── AR Camera
│   ├── AR Tracked Image Manager
│   └── ImageTracker component
│
├── GameManager
│   ├── BattleManager component
│   ├── MovesManager component
│   ├── SwipeProjectileLauncher (optional)
│   └── MovePrefabGenerator (optional)
│
├── Canvas
│   ├── BattleStatusText
│   ├── CharizardHPText
│   ├── PikachuHPText
│   ├── CharizardAttackButton
│   ├── PikachuAttackButton
│   └── ResetButton
│
└── Lighting (standard scene lighting)
```

## 🔗 Component Dependencies

```
ImageTracker
    ↓ spawns
PokemonController (on prefabs)
    ↓ detected by
BattleManager
    ↓ uses
MovesManager (move data)
    ↓ spawns
MoveProjectile (on move prefabs)
    ↓ damages
PokemonController (target)
```

## ⚙️ Configuration Flow

```
1. Create Reference Image Library
   └─ Add Pokemon card images

2. Setup AR Session Origin
   └─ Assign library to AR Tracked Image Manager

3. Create Pokemon Prefabs
   ├─ Add PokemonController
   ├─ Configure HP/Damage
   └─ Assign to ImageTracker

4. Create Move Prefabs
   ├─ Add MoveProjectile
   ├─ Configure damage/speed
   └─ Assign to BattleManager

5. Setup Battle Manager
   ├─ Reference move prefabs
   ├─ Configure throw settings
   └─ Link UI elements

6. Test in AR Simulation or Device
```

This architecture provides a modular, extensible system for AR Pokemon battles with throwable moves!
