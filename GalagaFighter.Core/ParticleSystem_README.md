# Particle Emitter System

This particle emitter system provides a flexible and efficient way to create visual effects in your Galaga Fighter game. It includes particles for explosions, trails, smoke, sparks, ice effects, and more.

## Core Components

### 1. Particle Class
The `Particle` class represents individual particles with properties like:
- Lifetime and fade effects
- Size interpolation (start size to end size)
- Color interpolation (start color to end color)
- Physics (gravity, drag, acceleration)
- Automatic cleanup when lifetime expires

### 2. ParticleEmitter Class
The `ParticleEmitter` class manages groups of particles and handles:
- Different emission shapes (Point, Circle, Rectangle, Line, Cone)
- Configurable emission rates and particle limits
- Automatic particle spawning and cleanup
- Burst emission capabilities

### 3. ParticleEmitterConfig Class
Configuration object that defines emitter behavior:
- Emission properties (shape, rate, direction)
- Particle properties (lifetime, size, color, physics)
- Duration and looping behavior

## Quick Start

### 1. Add Particle Service to Your Game

```csharp
// In your Game constructor or initialization
var particleService = new ParticleService(objectService);
Registry.Register<IParticleService>(particleService);

// In your Game.Update() method
particleService.UpdateParticleEmitters(this);
```

### 2. Create Basic Effects

```csharp
// Get the particle service
var particleService = Registry.Get<IParticleService>();

// Create an explosion when something explodes
particleService.CreateExplosion(explosionPosition, ownerId, Color.Orange, 1.5f);

// Create a projectile trail
var trailEmitter = particleService.CreateProjectileTrail(projectilePosition, projectileId, Color.Yellow);

// Create impact sparks when something hits
particleService.CreateSparks(impactPosition, ownerId, 10);
```

## Available Effects

### Built-in Effects (via ParticleService)

1. **Explosion** - Burst of particles expanding outward
2. **Projectile Trail** - Continuous trail following moving objects
3. **Engine Exhaust** - Continuous exhaust particles for ships
4. **Impact** - Directional particles when objects collide
5. **Sparks** - Brief spark burst with gravity
6. **Smoke** - Rising smoke particles that expand
7. **Magical Effect** - Floating magical particles
8. **Ice Crystals** - Ice particles for ice effects

### Custom Effects

```csharp
var config = new ParticleEmitterConfig
{
    Shape = EmissionShape.Circle,
    EmissionRate = 50f,
    MaxParticles = 30,
    ParticleLifetime = 2f,
    ParticleStartColor = Color.Red,
    ParticleEndColor = Color.Yellow,
    // ... more configuration
};

var customEmitter = particleService.CreateCustomEmitter(position, ownerId, config, customSprite);
```

## Integration Examples

### Adding Particles to Projectiles

```csharp
public class MyProjectile : Projectile
{
    private ParticleEmitter _trailEmitter;
    
    public MyProjectile(...) : base(...)
    {
        var particleService = Registry.Get<IParticleService>();
        _trailEmitter = particleService.CreateProjectileTrail(Center, Id, Color.Yellow);
    }
    
    public override void Update(Game game)
    {
        base.Update(game);
        
        // Update trail position
        if (_trailEmitter?.IsActive == true)
        {
            _trailEmitter.MoveTo(Center.X, Center.Y);
        }
    }
    
    public void OnDestroy()
    {
        var particleService = Registry.Get<IParticleService>();
        particleService.CreateExplosion(Center, Owner, Color.Orange);
        
        // Stop the trail
        _trailEmitter?.StopEmission();
    }
}
```

### Adding Particles to Collision Detection

```csharp
private void HandleProjectileCollision(Projectile projectile, GameObject target)
{
    var particleService = Registry.Get<IParticleService>();
    
    // Create impact effect
    Vector2 impactDirection = Vector2.Normalize(projectile.Speed);
    particleService.CreateImpact(projectile.Center, impactDirection, projectile.Owner);
    
    // Create type-specific effects
    if (projectile is IceProjectile)
    {
        particleService.CreateIceCrystals(projectile.Center, projectile.Owner);
    }
    else
    {
        particleService.CreateSparks(projectile.Center, projectile.Owner, 8);
    }
}
```

### Adding Continuous Effects to Players

```csharp
public class Player : GameObject
{
    private ParticleEmitter _engineExhaust;
    
    private void CreateEngineExhaust()
    {
        var particleService = Registry.Get<IParticleService>();
        Vector2 exhaustPosition = new Vector2(Center.X, Rect.Y + Rect.Height + 5f);
        _engineExhaust = particleService.CreateEngineExhaust(exhaustPosition, Id, IsPlayer1);
    }
    
    public override void Update(Game game)
    {
        base.Update(game);
        
        // Update exhaust position
        if (_engineExhaust?.IsActive == true)
        {
            Vector2 exhaustPos = new Vector2(Center.X, Rect.Y + Rect.Height + 5f);
            _engineExhaust.MoveTo(exhaustPos.X, exhaustPos.Y);
            
            // Control exhaust based on player movement
            if (IsMoving)
                _engineExhaust.StartEmission();
            else
                _engineExhaust.StopEmission();
        }
    }
}
```

## Performance Considerations

1. **Particle Limits**: Each emitter has a `MaxParticles` limit to prevent performance issues
2. **Automatic Cleanup**: Particles and emitters automatically clean themselves up
3. **Efficient Sprites**: Use the built-in sprite generation for optimal performance
4. **Smart Emission**: Emitters stop creating particles when not needed

## Advanced Features

### Custom Particle Sprites

```csharp
// Use built-in sprite generation
var sparkSprite = SpriteGenerationService.CreateSparkParticleSprite(6, 2, Color.Yellow);
var smokeSprite = SpriteGenerationService.CreateSmokeParticleSprite(8, Color.Gray);

// Or create custom sprites
var customSprite = new SpriteWrapper("path/to/particle.png");
```

### Physics Effects

```csharp
var config = new ParticleEmitterConfig
{
    UseGravity = true,
    GravityStrength = 200f,
    ParticleDrag = 1.5f,
    ParticleAcceleration = new Vector2(10f, 0f)
};
```

### Emission Shapes

- **Point**: Emit from a single point
- **Circle**: Emit randomly around a circle
- **Rectangle**: Emit randomly within a rectangle
- **Line**: Emit along a line
- **Cone**: Emit in a directional cone

## Debug Features

In debug mode, emitters will draw their emission shapes to help with positioning and tuning.

## Common Patterns

1. **One-shot Effects**: Use `Duration` with `Loop = false` for explosions, impacts
2. **Continuous Effects**: Use `Duration = -1` with `Loop = true` for trails, exhaust
3. **Timed Effects**: Use specific `Duration` values for temporary effects
4. **Burst Effects**: Use `emitter.Burst(count)` for instant particle creation

This particle system integrates seamlessly with your existing game architecture and provides the visual flair needed for an engaging space shooter experience!