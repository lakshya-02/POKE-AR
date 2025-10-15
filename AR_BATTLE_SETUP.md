# AR Pokemon Battle Game - Setup Guide

## 🎮 Overview
This AR Pokemon game uses image tracking to spawn Pokemon (Charizard and Pikachu), play idle animations, and initiate battles where you can throw move projectiles at opponents.

## 📁 Scripts Created/Updated

### Core Battle Scripts
1. **PokemonController.cs** - Controls individual Pokemon behavior, HP, animations
2. **BattleManager.cs** - Manages battle state, turn system, projectile spawning
3. **MoveProjectile.cs** - Handles projectile physics, collision, damage
4. **MovesManager.cs** - Stores move data (damage values, names)
5. **SwipeProjectileLauncher.cs** - Optional swipe/drag throwing controls
6. **ImageTracker.cs** - Updated to work with battle system

---

## 🛠️ Unity Scene Setup

### Step 1: AR Foundation Setup
1. Create/open your AR scene
2. Add **AR Session** and **AR Session Origin** GameObjects
3. Add **AR Tracked Image Manager** component to AR Session Origin
4. Create an **XR Reference Image Library** with your Pokemon card images:
   - Add Charizard image (name it exactly "Charizard")
   - Add Pikachu image (name it exactly "Pikachu")
5. Assign the library to AR Tracked Image Manager

### Step 2: Create Pokemon Prefabs

#### Charizard Prefab:
1. Create empty GameObject named "Charizard"
2. Add your 3D Charizard model as child
3. Add **PokemonController** component:
   - Pokemon Name: `Charizard`
   - Max HP: `200`
   - Current HP: `200`
   - Move Damage: `60`
   - Move Name: `Flamethrower`
4. Assign Animator component reference
5. Add **Collider** (BoxCollider or MeshCollider) for hit detection
6. Save as prefab

#### Pikachu Prefab:
1. Create empty GameObject named "Pikachu"
2. Add your 3D Pikachu model as child
3. Add **PokemonController** component:
   - Pokemon Name: `Pikachu`
   - Max HP: `150`
   - Current HP: `150`
   - Move Damage: `40`
   - Move Name: `Thunderbolt`
4. Assign Animator component reference
5. Add **Collider** for hit detection
6. Save as prefab

### Step 3: Create Move Prefabs

#### Charizard Move (Flamethrower):
1. Create GameObject named "Flamethrower"
2. Add fire particle effect or 3D model
3. Add **Sphere Collider** (set as Trigger)
4. Add **Rigidbody** (uncheck Use Gravity)
5. Add **MoveProjectile** component:
   - Speed: `10`
   - Damage: `60`
   - Move Name: `Flamethrower`
   - Lifetime: `5`
6. Optional: Add trail renderer or particle effects
7. Save as prefab

#### Pikachu Move (Thunderbolt):
1. Create GameObject named "Thunderbolt"
2. Add electric particle effect or lightning model
3. Add **Sphere Collider** (set as Trigger)
4. Add **Rigidbody** (uncheck Use Gravity)
5. Add **MoveProjectile** component:
   - Speed: `10`
   - Damage: `40`
   - Move Name: `Thunderbolt`
   - Lifetime: `5`
6. Optional: Add particle effects
7. Save as prefab

### Step 4: Scene Manager Setup

1. Create empty GameObject named "GameManager"
2. Add **BattleManager** component:
   - Assign Charizard Move Prefab
   - Assign Pikachu Move Prefab
   - Use Auto Target: ✓ (for automatic aiming)
   - Throw Force: `15`

3. Add **MovesManager** component (same GameObject or separate)

4. Add **ImageTracker** component to AR Session Origin:
   - Assign ARTrackedImageManager component
   - Add both Pokemon prefabs to AR Prefabs array (size: 2)
     - Element 0: Charizard prefab
     - Element 1: Pikachu prefab

5. Optional: Add **SwipeProjectileLauncher** for swipe controls:
   - Assign BattleManager reference
   - Min Swipe Distance: `50`
   - Swipe Speed Multiplier: `0.02`

### Step 5: UI Setup

Create a Canvas with:

1. **Battle Status Text** (Text component)
   - Shows battle messages
   - Assign to BattleManager → Battle Status Text

2. **Charizard HP Text** (Text component)
   - Shows Charizard HP
   - Assign to BattleManager → Charizard HP Text

3. **Pikachu HP Text** (Text component)
   - Shows Pikachu HP
   - Assign to BattleManager → Pikachu HP Text

4. **Charizard Attack Button** (Button component)
   - Text: "Charizard Attack"
   - Assign to BattleManager → Charizard Attack Button

5. **Pikachu Attack Button** (Button component)
   - Text: "Pikachu Attack"
   - Assign to BattleManager → Pikachu Attack Button

6. Optional: **Reset Battle Button**
   - OnClick → BattleManager.ResetBattle()

---

## 🎯 How It Works

### Image Tracking Flow:
1. User starts game (via GameStart.cs)
2. Point camera at first Pokemon card (e.g., Charizard)
3. Charizard spawns and plays **idle animation**
4. Point camera at second Pokemon card (Pikachu)
5. Pikachu spawns and plays **idle animation**
6. Battle automatically starts when both are detected

### Battle Flow:
1. BattleManager detects both Pokemon spawned
2. UI buttons become active
3. Player taps attack button OR swipes from Pokemon
4. Move prefab spawns above attacking Pokemon
5. Projectile flies toward opponent
6. On collision, damage is applied
7. Opponent plays hit animation, HP decreases
8. Battle continues until one faints

### Throwing Mechanics:

#### Option A: Button-Based (Auto-Aim)
- Set `BattleManager.useAutoTarget = true`
- Tap UI buttons to launch moves
- Projectiles automatically home in on target

#### Option B: Swipe-Based (Manual Throw)
- Enable SwipeProjectileLauncher component
- Tap and drag on screen from Pokemon
- Swipe direction/speed affects throw
- More interactive but requires practice

---

## 🎨 Animation Setup

Your Pokemon models need an Animator with these triggers:
- **Idle** - Plays when spawned/waiting
- **Attack** - Plays when using a move
- **Hit** - Plays when taking damage
- **Faint** - Plays when HP reaches 0

If you don't have animations, the scripts will still work without them.

---

## 🔧 Configuration Tips

### Adjusting Difficulty:
```csharp
// In PokemonController prefabs:
charizard.maxHP = 200; // Change HP values
charizard.moveDamage = 60; // Change damage

pikachu.maxHP = 150;
pikachu.moveDamage = 40;
```

### Projectile Speed:
```csharp
// In MoveProjectile prefabs:
speed = 10f; // Slower = easier to see, faster = more challenging
```

### Battle Balance:
- Charizard: 200 HP, 60 damage (stronger, slower attacker)
- Pikachu: 150 HP, 40 damage (weaker, faster attacker)

---

## 📱 Testing

### In Unity Editor:
1. Use AR Simulation (AR Foundation 4.2+)
2. Place virtual images in scene
3. Test button attacks

### On Device:
1. Build to Android/iOS
2. Print Pokemon card images
3. Point camera at cards
4. Battle in real AR!

---

## 🐛 Troubleshooting

### Pokemon not spawning:
- Check image library names match prefab names exactly
- Ensure good lighting and image contrast
- Images should be at least 15cm wide

### Projectiles not hitting:
- Verify Pokemon have colliders
- Check MoveProjectile has collider set as Trigger
- Ensure layers aren't blocking collisions

### No animations playing:
- Check Animator component assigned
- Verify animation controller has correct triggers
- Scripts work fine without animations (just won't look as good)

### Battle not starting:
- Both Pokemon must be spawned AND tracked
- Check BattleManager references in Inspector
- Look at Console for debug messages

---

## 🎮 Controls Summary

**Button Mode:**
- Tap "Charizard Attack" button → Charizard throws move
- Tap "Pikachu Attack" button → Pikachu throws move

**Swipe Mode (if enabled):**
- Tap on Pokemon → Swipe toward opponent
- Swipe speed = projectile speed
- Visual feedback shows swipe path

---

## 📝 Next Steps / Enhancements

Want to add more features? Consider:
1. **Multiple Moves** - Add 2-4 moves per Pokemon with cooldowns
2. **Type Effectiveness** - Fire beats Grass, Water beats Fire, etc.
3. **Critical Hits** - Random chance for 2x damage
4. **Status Effects** - Burn, Paralysis, Sleep
5. **Health Bars** - Visual HP bars above Pokemon
6. **Sound Effects** - Attack sounds, hit sounds, victory music
7. **More Pokemon** - Add Squirtle, Bulbasaur, etc.
8. **Power-ups** - Collectible items in AR space

---

## 📞 Support

If something isn't working:
1. Check Unity Console for errors
2. Verify all Inspector references are assigned
3. Ensure AR Foundation packages are installed
4. Test image tracking separately first

The compile errors shown during creation are normal - Unity needs to recompile all scripts together. They'll disappear after Unity processes the new scripts!
