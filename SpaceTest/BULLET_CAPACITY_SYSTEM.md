# Bullet Capacity System

## Overview
The fire rate power-up system has been completely redesigned to use a **bullet capacity limit** instead of increasing fire rate. This creates more tactical gameplay where players must manage their bullet usage strategically rather than just firing as fast as possible.

## New Mechanics

### **?? Bullet Limit System**
- **Default capacity**: 8 bullets on screen simultaneously
- **Default fire rate**: Fast base rate (equivalent to 5 power-ups in old system)  
- **Power-up effect**: Each power-up adds **+2 bullet capacity**
- **Shooting limitation**: Cannot shoot when at maximum bullet capacity

### **? Fast Base Fire Rate**
- Fire rate set to `0.35f * 0.8^5 ? 0.11f` (very fast)
- No more fire rate improvements from power-ups
- Consistent rapid-fire experience for all players
- Focus shifts from "how fast can I shoot" to "how many bullets can I maintain"

### **?? Bullet Capacity Progression**
- **Default**: 8 bullets maximum
- **1 Power-up**: 10 bullets maximum (+2)
- **2 Power-ups**: 12 bullets maximum (+4)
- **3 Power-ups**: 14 bullets maximum (+6)
- **And so on...**

## Technical Implementation

### **Bullet Counting System**
```csharp
private int GetActiveBulletCount(Game game)
{
    return game.GetGameObjects()
        .OfType<Projectile>()
        .Where(p => p.Owner == this && p is NormalProjectile)
        .Count();
}
```

### **Shooting Logic**
```csharp
int activeBullets = GetActiveBulletCount(game);
bool canShoot = Raylib.IsKeyDown(shootKey) && 
               FireTimer >= FireRate && 
               activeBullets < MaxBullets;
```

### **Power-up Effect**
```csharp
case PowerUpType.BulletCapacity:
    MaxBullets += bulletsPerPowerUp; // +2 bullets
    break;
```

## UI Feedback System

### **Real-time Bullet Counter**
- **Display**: "P1 Bullets: 5/8" format
- **Color coding**: 
  - **Yellow**: Normal operation (can shoot)
  - **Red**: At maximum capacity (cannot shoot)

### **Visual Indicators**
- **Power-up sprite**: White diamond with "B" symbol
- **Clear feedback**: Players always know their bullet status
- **Status positioning**: Below health display

## Gameplay Impact

### **?? Strategic Advantages**
- **Burst vs Sustained**: Players choose between rapid bursts or sustained pressure
- **Accuracy Matters**: Missed shots reduce available firepower
- **Power-up Value**: Each bullet capacity upgrade significantly improves combat effectiveness

### **?? Tactical Considerations**
- **Timing Shots**: Players must time shots more carefully
- **Bullet Management**: Running out of bullets creates vulnerability windows
- **Power-up Priority**: Bullet capacity becomes highly valuable upgrade

### **??? Defensive Play**
- **Bullet Blocking**: Opponents can potentially outlast bullet barrages
- **Counter-attack Windows**: When enemy is at bullet limit, they're vulnerable
- **Positioning**: More important since you can't always shoot immediately

## Balance Design

### **?? Core Philosophy**
- **Fast fire rate baseline**: Everyone gets responsive shooting
- **Capacity-based scaling**: Power-ups improve sustained damage output
- **Strategic depth**: Bullet management adds tactical layer

### **?? Progression Curve**
| Power-ups | Max Bullets | Improvement | Tactical Impact |
|-----------|-------------|-------------|------------------|
| 0         | 8           | Baseline    | Basic firepower  |
| 1         | 10          | +25%        | Noticeable boost |
| 2         | 12          | +50%        | Strong advantage |
| 3         | 14          | +75%        | Dominant firepower|

### **?? Balancing Factors**
- **Bullet travel time**: Bullets take time to cross screen
- **Miss penalty**: Wasted bullets reduce effective capacity
- **Recovery time**: Must wait for bullets to hit/expire before shooting more

## Comparison: Old vs New

### **Before (Fire Rate System)**
- Slow initial fire rate (0.35f)
- Each power-up: 20% faster shooting (×0.8)
- Linear improvement in DPS
- Eventually became machine-gun rapid fire
- No strategic depth beyond "shoot faster"

### **After (Bullet Capacity System)**  
- Fast base fire rate (0.11f)
- Each power-up: +2 bullet capacity
- Sustained damage improvement
- Bullet management adds tactical layer
- Strategic resource management

## Strategic Implications

### **?? Offensive Strategy**
```csharp
if (activeBullets < maxBullets * 0.5f) 
{
    // Good time for aggressive push
    FireBurst();
}
else 
{
    // Conserve bullets, wait for hits
    CalculatedShots();
}
```

### **??? Defensive Strategy**
```csharp
if (enemy.bulletCount >= enemy.maxBullets)
{
    // Enemy can't shoot - counter-attack window!
    AggressivePush();
}
```

## Benefits

### **?? Gameplay Benefits**
- **More tactical depth** - Bullet management vs pure speed
- **Consistent responsiveness** - Fast fire rate for everyone
- **Power-up value** - Each upgrade meaningfully improves capability
- **Skill expression** - Accuracy and timing matter more

### **?? Technical Benefits**
- **Clean implementation** - Simple counting system
- **Visual feedback** - Clear UI shows bullet status
- **Balanced scaling** - Linear capacity growth vs exponential fire rate
- **Performance** - No audio spam from ultra-rapid fire

This system transforms the simple "shoot faster" mechanic into a sophisticated resource management system that rewards accuracy, timing, and tactical thinking while maintaining the fast-paced action of the original game.