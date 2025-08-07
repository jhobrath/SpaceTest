# Modular Projectile Architecture

## Overview
The projectile system has been refactored from an enum-based approach to a proper inheritance hierarchy. This provides better separation of concerns and makes it extremely easy to add new projectile types.

## Benefits

### 1. **Decoupled Design**
- Each projectile type is its own class with encapsulated behavior
- No more if/switch statements scattered throughout the codebase
- Game.cs no longer needs to know about specific projectile behaviors

### 2. **Easy to Extend**
Adding a new projectile type now requires:
1. Create a new class inheriting from `Projectile`
2. Add the type to the `ProjectileType` enum
3. Add one line to the `ProjectileFactory`

That's it! No changes needed to Game.cs, Player.cs, or any other files.

### 3. **Self-Contained Behavior**
Each projectile type defines its own:
- `OnHit()` behavior (what happens when it hits a player)
- `Damage` amount
- `BlocksMovement` property (for walls)
- `DestroyOnHit` property
- `GetColor()` visual appearance
- `UpdateMovement()` custom movement patterns
- `Draw()` custom visual effects

## Example: Creating a New Projectile Type

```csharp
public class PoisonProjectile : Projectile
{
    public PoisonProjectile(Rectangle rect, float speed, Player owner) 
        : base(rect, speed, owner) { }

    public override int Damage => 5; // Less immediate damage

    public override void OnHit(Player target, Game game)
    {
        target.Health -= Damage;
        target.PoisonTimer = 10.0f; // Apply poison effect for 10 seconds
        game.PlayPoisonSound();
    }

    public override Color GetColor() => Color.Green;
}
```

Then just add `ProjectileType.Poison` to the enum and factory - done!

## Architecture Components

- **`Projectile`** (abstract base): Common behavior and virtual methods
- **`NormalProjectile`**: Standard projectile with damage + sound
- **`IceProjectile`**: No damage, applies slow effect
- **`WallProjectile`**: Blocks movement, sticks to edges
- **`ExplosiveProjectile`**: Area-of-effect damage with visual effects
- **`ProjectileFactory`**: Centralized creation logic