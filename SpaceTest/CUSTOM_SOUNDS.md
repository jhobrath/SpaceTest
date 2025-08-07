# Sound System - Custom Audio Files

## Overview
The game now supports loading custom WAV sound files for specific game actions, with fallbacks to procedurally generated sounds if files are not found. Individual sound volumes can be controlled for better audio balance.

## Required Sound Files

Place these sound files in one of the following directories:
- `SpaceTest/` (project root)
- `SpaceTest/sounds/`
- `SpaceTest/assets/`
- `SpaceTest/audio/`

### Files Needed:

1. **`projectile-default.wav`** - Plays when any projectile is fired
   - Used for: All projectile types (Normal, Ice, Wall, Explosive)
   - Trigger: Every time a player presses their shoot key
   - **Volume: 50%** (reduced to prevent audio overload during rapid fire)

2. **`player_freezing.wav`** - Plays when a player gets frozen by ice
   - Used for: Ice projectile hit effects
   - Trigger: When an ice projectile hits a player (applies slow effect)
   - **Volume: 100%**

3. **`plank_landing.wav`** - Plays when a wall projectile sticks to screen edge
   - Used for: Wall projectile sticking behavior
   - Trigger: When a brown/wall projectile reaches and sticks to the screen edge
   - **Volume: 100%**

## Sound Usage Map

| Game Action | Sound File | Volume | Fallback |
|-------------|------------|--------|----------|
| Any projectile fired | `projectile-default.wav` | **50%** | Generated beep (800Hz) |
| Player gets frozen | `player_freezing.wav` | 100% | Generated beep (1200Hz) |
| Wall projectile sticks | `plank_landing.wav` | 100% | Generated beep (150Hz) |
| Player takes damage | *Generated* | 100% | Generated beep (200Hz) |
| Power-up collected | *Generated* | 100% | Generated beep (600Hz) |

## Volume Control

The sound system now includes volume control for individual sounds:
- **Projectile sounds are played at 50% volume** to prevent audio overload during rapid fire sequences
- **All other sounds remain at 100% volume** for clear audio feedback
- Volume levels are applied to both custom sound files and generated fallback sounds

## File Specifications

- **Format**: WAV files (recommended)
- **Quality**: Any standard WAV format supported by Raylib
- **Length**: Keep sound effects short (0.1-0.5 seconds recommended)
- **Volume**: Create sounds at moderate levels (the game will adjust volume as needed)

## Fallback System

If any sound file cannot be found, the game will:
1. First try procedurally generated beep sounds (with same volume settings)
2. As a last resort, create silent placeholder sounds
3. Continue running without crashing

## Adding More Custom Sounds

To add more custom sound files, modify the `CreatePlaceholderSounds()` method in `Game.cs`:
hitSound = LoadSoundFile("your-custom-hit-sound.wav");
powerUpSound = LoadSoundFile("your-custom-powerup-sound.wav");
Then add volume control in the `SetSoundVolumes()` method:
Raylib.SetSoundVolume(hitSound, 0.8f);        // 80% volume
Raylib.SetSoundVolume(powerUpSound, 1.0f);    // 100% volume
## Debugging Sound Loading

Check the console output when the game starts. You'll see messages like:
- `"Loading sound: sounds/projectile-default.wav"` (success)
- `"Warning: Could not find sound file 'missing.wav'"` (file not found)
- `"Audio system initialized with mixed sound sources"` (some files loaded)
- `"Audio system initialized with generated fallback sounds"` (no files loaded)