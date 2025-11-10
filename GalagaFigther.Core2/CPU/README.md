# CPU AI System for GalagaFighter Core2

## Overview

This CPU AI system provides intelligent, configurable computer opponents for your game. The system is built using a **Strategy Pattern** architecture that makes it highly scalable and easy to extend with new behaviors.

## Architecture

### Core Components

- **CpuInputService**: Main entry point that integrates with the game's input system
- **CpuDecisionEngine**: Orchestrates multiple strategies to make final decisions  
- **ICpuStrategy**: Interface for individual AI behaviors
- **ThreatAnalyzer**: Analyzes incoming projectiles and calculates danger levels
- **CpuConfiguration**: Difficulty and behavior settings

### Strategy-Based Decision Making

The AI uses multiple strategies that execute in priority order:

1. **EmergencyEvasionStrategy** (Priority: 100)
   - Handles immediate life-threatening situations
   - Calculates optimal evasion directions
   - Overrides all other behaviors when in extreme danger

2. **ProjectileDodgeStrategy** (Priority: 80)  
   - Predictive dodging of incoming projectiles
   - Uses threat analysis to determine safe movement
   - Maintains shooting while dodging

3. **PowerUpCollectionStrategy** (Priority: 60)
   - Opportunistic power-up collection when safe
   - Calculates risk vs. reward for power-up acquisition
   - Only activates when not in immediate danger

4. **CombatPositioningStrategy** (Priority: 40)
   - Aggressive positioning for optimal shooting angles
   - Maintains safe distance while staying aligned with opponent
   - Balances offense and defense

5. **MovementPatternStrategy** (Priority: 20)
   - Fallback movement patterns and screen edge avoidance
   - Provides basic movement when no specific threats exist
   - Ensures CPU doesn't get stuck at screen edges

## Configuration System

### Difficulty Presets

```csharp
// Easy mode - forgiving for new players
var easyConfig = CpuConfiguration.Easy;

// Normal mode - balanced challenge
var normalConfig = CpuConfiguration.Normal;

// Hard mode - skilled challenge
var hardConfig = CpuConfiguration.Hard;

// Expert mode - maximum difficulty
var expertConfig = CpuConfiguration.Expert;
```

### Custom Configuration

```csharp
var customConfig = new CpuConfiguration
{
    AggressionLevel = 0.8f,      // How aggressive the CPU is (0.0-1.0)
    ReactionTime = 0.9f,         // How quickly CPU reacts (0.0-1.0)
    AccuracyLevel = 0.7f,        // Shooting accuracy (0.0-1.0)
    RiskTaking = 0.6f,           // Willingness to take risks (0.0-1.0)
    DecisionUpdateRate = 0.08f,   // Seconds between decisions
    PowerUpPriorityWeight = 0.7f  // How much CPU values power-ups
};

cpuInputService.SetConfiguration(customConfig);
```

## Usage

### Basic Setup

```csharp
// In your game initialization
var cpuService = Registry.Get<ICpuInputService>();

// Set the CPU player
cpuService.SetPlayer(player2);

// Configure difficulty (optional - defaults to Normal)
cpuService.SetConfiguration(CpuConfiguration.Hard);
```

### In Game Loop

```csharp
// In your update loop
cpuService.Update(frameTime);

// Get debug information (optional)
var debugInfo = cpuService.GetDebugInfo();
DebugWriter.Write(debugInfo);
```

## Key Features

### ?? **Intelligent Threat Analysis**
- Predicts projectile trajectories
- Calculates collision probabilities  
- Determines optimal evasion paths
- Considers multiple threats simultaneously

### ?? **Smart Decision Making**
- Priority-based strategy execution
- Context-aware behavior selection
- Configurable reaction times and accuracy
- Risk assessment for power-up collection

### ?? **Scalable Difficulty**
- Multiple preset difficulty levels
- Fine-grained configuration options
- Performance tracking and analytics
- Adaptive behavior based on settings

### ?? **Easily Extensible**
- Clean strategy pattern architecture
- Simple interface for adding new behaviors
- Modular threat analysis system
- Configuration-driven customization

## Performance Characteristics

- **Decision Rate**: 3-20 decisions per second (configurable)
- **Threat Analysis**: Real-time projectile tracking
- **Memory Efficient**: Minimal object allocation per frame
- **CPU Optimized**: Lightweight calculations with early exits

## Debugging and Tuning

### Debug Information

The system provides comprehensive debug output:

```
CPU: [Emergency Evasion] Avoiding 0.85 danger level threat
Config: 0.7A/0.8R/0.6Acc  
Perf: 8.5 DPS, 423 total decisions
```

### Performance Monitoring

- **DPS**: Decisions per second (should match configuration)
- **Strategy Usage**: Which strategies are being executed most
- **Reaction Accuracy**: How well the CPU is performing

## Adding New Strategies

Creating a custom strategy is simple:

```csharp
public class MyCustomStrategy : ICpuStrategy
{
    public string Name => "My Custom Strategy";
    public int Priority => 70; // Higher = executed first

    public bool ShouldExecute(CpuContext context)
    {
        // Return true when this strategy should run
        return context.SomeCondition;
    }

    public CpuDecision Execute(CpuContext context)
    {
        return new CpuDecision
        {
            MoveUp = true,
            Shoot = true,
            Reason = "My custom behavior"
        };
    }
}
```

Then register it in the CpuDecisionEngine constructor.

## Future Enhancements

- **Machine Learning Integration**: Train strategies on player behavior
- **Personality System**: Different AI "personalities" with unique behaviors  
- **Dynamic Difficulty**: Adjust challenge based on player performance
- **Team AI**: Coordinate multiple CPU players
- **Advanced Prediction**: More sophisticated trajectory prediction

---

**Note**: This AI system is designed specifically for the GalagaFighter Core2 project structure and integrates with its existing input and object management systems.