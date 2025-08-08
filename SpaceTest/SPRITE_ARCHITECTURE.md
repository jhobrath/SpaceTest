# Sprite System Architecture

## Overview
The sprite system generates visual assets procedurally using Raylib's drawing functions, similar to how we generate sound effects. This approach eliminates the need for external image files and ensures consistent visual style.

## Benefits

### ?? **Procedurally Generated Sprites**
- **No external files needed** - All sprites are generated at runtime
- **Consistent style** - Unified visual theme across all game elements
- **Scalable** - Sprites can be generated at any size
- **Lightweight** - Small memory footprint compared to bitmap images

### ?? **Visual Enhancements Added**
- **Ship Sprites**: Detailed spaceship designs with cockpit, wings, and engines
- **Projectile Sprites**: Unique visuals for each projectile type
- **Power-up Sprites**: Rotating diamonds with type indicators
- **Dynamic Effects**: Engine trails, glow effects, and pulsing animations

## Sprite Types

### **?? Player Ships**
- **Player 1**: Blue/SkyBlue ship with yellow engine (40x80 pixels, properly scaled for fullscreen)
- **Player 2**: Red/Maroon ship with orange engine (40x80 pixels, properly scaled for fullscreen)  
- **Features**: Cockpit nose, wings, engine exhaust, detail lines - all elements scale proportionally
- **Effects**: Blue tint when slowed, enhanced engine trails when moving

### **?? Projectiles**
- **Normal**: Yellow bullet with white tip (20x10 pixels - doubled from original 10x5)
- **Ice**: Six-sided horizontally elongated diamond crystal with crystalline highlights (20x20 pixels - original size)
- **Wall**: Brown brick pattern with maroon borders (100x10 pixels - original size)
- **Explosive**: Orange/red explosive with pulsing glow effect (original size)

### **?? Power-ups**
- **Visual**: Rotating diamond shapes with inner glow
- **Colors**: White (FireRate), Blue (IceShot), Brown (Wall)
- **Labels**: F, I, W letters for easy identification
- **Effects**: Rotation animation and colored glow halos

## Architecture Components

### **`SpriteGenerator`** (Static Class)
- `CreatePlayerShip()` - Generates ship sprites with different colors
- `CreateProjectileSprite()` - Creates projectile visuals by type
- `CreatePowerUpSprite()` - Generates rotating diamond power-ups

### **Enhanced Game Objects**
- **`Player`**: Now uses ship sprites with effects
- **`Projectile`**: Base class supports both sprites and color fallbacks
- **`PowerUp`**: Rotating sprites with glow effects

## Visual Effects Added

### **?? Engine Trails**
- Appear when ships are moving
- Orange particle trail behind engines
- Different positioning for Player 1 vs Player 2

### **?? Status Effects**
- Blue tint overlay when slowed by ice
- Visual feedback for game state changes

### **?? Special Effects**
- Pulsing glow on explosive projectiles
- Rotating power-ups for visibility
- Glow halos around collectibles

## Fallback System
- **Sprite Loading**: If sprite generation fails, falls back to colored rectangles
- **Robustness**: Game continues working even if visual enhancements fail
- **Debug Friendly**: Easy to identify when sprites vs fallbacks are used

## Future Expansion
Adding new visual elements is now extremely easy:
- Create new generation methods in `SpriteGenerator`
- Add sprite loading to constructors
- Implement custom `Draw()` methods for special effects
- All assets remain code-based and version-controlled