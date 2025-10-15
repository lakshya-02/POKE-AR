# 🎮 AR Pokemon Battle - Quick Start

## What You Get

A complete AR Pokemon battle system where:
1. **Image Tracking** spawns Pokemon when you point at cards
2. **Idle Animations** play when Pokemon are detected
3. **Battle Mode** activates when both Pokemon are present
4. **Throwable Moves** are projectiles you launch at opponents
5. **Damage System** tracks HP and determines winner

---

## 🚀 Quick Setup (5 Minutes)

### 1. Pokemon Prefabs Setup
**Charizard:**
- Drag your Charizard 3D model into scene
- Add Component → `PokemonController`
- Set: Name=`Charizard`, HP=`200`, Damage=`60`, Move=`Flamethrower`
- Add BoxCollider
- Save as Prefab
- Delete from scene

**Pikachu:**
- Drag your Pikachu 3D model into scene
- Add Component → `PokemonController`
- Set: Name=`Pikachu`, HP=`150`, Damage=`40`, Move=`Thunderbolt`
- Add BoxCollider
- Save as Prefab
- Delete from scene

### 2. Move Prefabs (Option A: Quick Auto-Generate)
1. Create empty GameObject: "GameManager"
2. Add Component → `MovePrefabGenerator`
3. Right-click component → **Create Move Prefabs**
4. Done! Prefabs created automatically

### 2. Move Prefabs (Option B: Manual)
Create two spheres with:
- SphereCollider (Is Trigger: ✓)
- Rigidbody (Use Gravity: ✗)
- MoveProjectile component
- Save as prefabs

### 3. Battle Manager
On GameManager GameObject:
1. Add Component → `BattleManager`
2. Assign the two move prefabs
3. Add Component → `MovesManager`
4. Done!

### 4. Image Tracking
On AR Session Origin:
1. Add Component → `ImageTracker`
2. Assign `ARTrackedImageManager`
3. Set AR Prefabs array size to 2
4. Drag Charizard prefab to slot 0
5. Drag Pikachu prefab to slot 1

### 5. Create Reference Images
1. Create → XR → Reference Image Library
2. Add your Charizard card image (name it "Charizard")
3. Add your Pikachu card image (name it "Pikachu")
4. Assign library to AR Tracked Image Manager

---

## 🎯 Testing

### In Editor (with AR Simulation):
1. Enable AR Simulation in XR settings
2. Press Play
3. Pokemon should spawn in simulated environment
4. Click attack buttons to test projectiles

### On Device:
1. Build and run on Android/iOS
2. Point camera at Charizard card → Spawns
3. Point at Pikachu card → Spawns
4. Battle starts automatically!
5. Tap attack buttons to throw moves

---

## 🎮 Two Control Modes

### Button Mode (Default - Easiest)
- `BattleManager.useAutoTarget = true`
- Tap UI button = auto-aimed projectile
- Perfect for beginners

### Swipe Mode (Advanced)
- Add Component → `SwipeProjectileLauncher`
- Tap Pokemon → swipe toward opponent
- More interactive!

---

## 📁 Scripts Reference

| Script | Purpose |
|--------|---------|
| **PokemonController** | Individual Pokemon behavior, HP, animations |
| **BattleManager** | Battle logic, turn system, projectile spawning |
| **MoveProjectile** | Projectile physics and damage |
| **MovesManager** | Move data storage (damage, names) |
| **ImageTracker** | AR image detection (updated) |
| **SwipeProjectileLauncher** | Optional swipe controls |
| **MovePrefabGenerator** | Quick prefab creation helper |

---

## 🔧 Common Adjustments

### Change Damage:
```csharp
// In PokemonController component on prefabs:
Charizard → Move Damage: 60
Pikachu → Move Damage: 40
```

### Change HP:
```csharp
Charizard → Max HP: 200
Pikachu → Max HP: 150
```

### Projectile Speed:
```csharp
// In MoveProjectile component on move prefabs:
Speed: 10-15 (slower = easier to see)
```

---

## 🎨 Animation Setup (Optional)

If you have animations, add Animator with triggers:
- `Idle` - Standing animation
- `Attack` - Using move
- `Hit` - Taking damage  
- `Faint` - Knocked out

**No animations?** Scripts still work fine!

---

## ✅ Checklist

- [ ] AR Foundation installed
- [ ] Pokemon prefabs with PokemonController
- [ ] Move prefabs with MoveProjectile
- [ ] BattleManager on GameManager
- [ ] ImageTracker on AR Session Origin
- [ ] Reference Image Library created
- [ ] UI Canvas with buttons (optional)
- [ ] Both Pokemon have colliders

---

## 🐛 Quick Fixes

**Pokemon not spawning?**
→ Check image library names match prefab names exactly

**Projectiles pass through?**
→ Add colliders to Pokemon, set move colliders as Trigger

**No battle starting?**
→ Both Pokemon must be visible simultaneously

**Can't see projectiles?**
→ They might be moving too fast, reduce speed

---

## 📖 Full Documentation

See `AR_BATTLE_SETUP.md` for complete details including:
- UI setup instructions
- Advanced features
- Troubleshooting guide
- Enhancement ideas

---

## 🎉 You're Ready!

Your AR Pokemon battle system is complete! The flow is:

1. Point camera at first Pokemon card → **Spawns + Idle animation**
2. Point at second card → **Spawns + Idle animation**  
3. Battle automatically starts → **UI activates**
4. Tap attack button → **Move prefab spawns and flies**
5. Projectile hits → **Damage applied, HP decreases**
6. Fight until one faints → **Winner declared!**

Have fun battling! 🔥⚡
