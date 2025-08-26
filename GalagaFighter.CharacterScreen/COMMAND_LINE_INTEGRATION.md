# Command Line Integration System

## Overview
This system allows the CharacterScreen project to pass selected character and effect data to GalagaFighter.Core via command line arguments.

## Implementation

### 1. CommandLineArgumentService
- **File**: `GalagaFighter.CharacterScreen/Services/CommandLineArgumentService.cs`
- **Purpose**: Formats player selection data into command line argument strings
- **Format**: `<ColorName>,<Augment>,<Health>,<Speed>,<FireRate>,<Damage>,<Shield>`
- **Example**: `"SkyBlue,Ricochet,100.0,1.0,1.0,1.0,1.0"`

### 2. CharacterSelectionGame Updates
- **File**: `GalagaFighter.CharacterScreen/CharacterSelectionGame.cs`
- **Changes**: 
  - Added `ICommandLineArgumentService` dependency
  - Modified `StartMainGame()` to format arguments and launch Core executable
  - Added `FindCoreExecutable()` to locate the Core game executable

### 3. Process Launch System
- Uses `ProcessStartInfo` to launch GalagaFighter.Core with arguments
- Searches for executable in common build output locations
- Passes formatted arguments as `--player1 "..." --player2 "..."`

## Usage Example

When players complete their selections in CharacterScreen:
- Player 1: Azure Wing ship + Ricochet effect
- Player 2: Crimson Hawk ship + Splitter effect

The system generates:
```
GalagaFighter.Core.exe --player1 "SkyBlue,Ricochet,100.0,1.0,1.0,1.0,1.0" --player2 "Red,Splitter,90.0,1.0,1.2,1.1,0.9"
```

## Color Mapping
The system maps Raylib Color objects to string names:
- `Color.SkyBlue` ? `"SkyBlue"`
- `Color.Red` ? `"Red"`
- `Color.Lime` ? `"Lime"`
- Custom colors ? `"RGB_R_G_B"` format

## Effect Mapping
Effect class names are simplified:
- `"GalagaFighter.Core.Models.Effects.RicochetEffect"` ? `"Ricochet"`
- `"SurpriseShotEffect"` ? `"SurpriseShot"`

## Integration Points
1. **Character Selection Completion**: When both players finish selections and press SPACE
2. **Executable Discovery**: Automatic search in build output directories
3. **Process Launch**: Clean handoff from CharacterScreen to Core game

## Testing
- Added `CommandLineArgumentTest.cs` for verification
- Integrated test into Program.cs startup
- Shows formatted arguments for debugging

## Benefits
- Clean separation between projects
- No direct dependencies from Core to CharacterScreen
- Simple text-based communication protocol
- Easy to extend with additional parameters