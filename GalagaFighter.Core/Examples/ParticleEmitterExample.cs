using GalagaFighter.Core;
using GalagaFighter.Core.Models.Particles;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Examples
{
    /// <summary>
    /// Example class showing how to use the particle emitter system
    /// </summary>
    public static class ParticleEmitterExample
    {
        /// <summary>
        /// Example: Create explosion effect when projectile hits a target
        /// Call this when a projectile collides with something
        /// </summary>
        public static void CreateProjectileExplosion(IParticleService particleService, Vector2 explosionPosition, 
                                                    Guid ownerId, Color? customColor = null)
        {
            // Create an explosion at the collision point
            Color explosionColor = customColor ?? Color.Orange;
            particleService.CreateExplosion(explosionPosition, ownerId, explosionColor, 1.5f);
            
            // Add some sparks for extra effect
            particleService.CreateSparks(explosionPosition, ownerId, 15);
        }

        /// <summary>
        /// Example: Add continuous engine exhaust to a player ship
        /// This should be called when creating a player or when they start moving
        /// </summary>
        public static ParticleEmitter AddEngineExhaustToPlayer(IParticleService particleService, 
                                                              GameObject player, bool isPlayer1)
        {
            // Position the exhaust at the bottom center of the player ship
            Vector2 exhaustPosition = new Vector2(
                player.Center.X, 
                player.Rect.Y + player.Rect.Height + 5f
            );
            
            var emitter = particleService.CreateEngineExhaust(exhaustPosition, player.Id, isPlayer1);
            
            // Note: You'll need to update the emitter position in your player's Update method
            // emitter.MoveTo(player.Center.X, player.Rect.Y + player.Rect.Height + 5f);
            
            return emitter;
        }

        /// <summary>
        /// Example: Create a projectile trail that follows a projectile
        /// Call this when creating a projectile that should have a trail
        /// </summary>
        public static ParticleEmitter AddProjectileTrail(IParticleService particleService, 
                                                        GameObject projectile, Color trailColor)
        {
            var emitter = particleService.CreateProjectileTrail(projectile.Center, projectile.Id, trailColor);
            
            // Note: You'll need to update the emitter position in your projectile's Update method
            // emitter.MoveTo(projectile.Center.X, projectile.Center.Y);
            
            return emitter;
        }

        /// <summary>
        /// Example: Create impact effect when something hits a wall or shield
        /// </summary>
        public static void CreateImpactEffect(IParticleService particleService, Vector2 impactPosition, 
                                            Vector2 impactDirection, Guid ownerId)
        {
            // Create impact particles
            particleService.CreateImpact(impactPosition, impactDirection, ownerId);
            
            // Add some smoke for lingering effect
            particleService.CreateSmoke(impactPosition, ownerId, 1f);
        }

        /// <summary>
        /// Example: Create ice shatter effect when ice projectile hits something
        /// </summary>
        public static void CreateIceShatterEffect(IParticleService particleService, Vector2 position, Guid ownerId)
        {
            // Create ice crystals
            particleService.CreateIceCrystals(position, ownerId);
            
            // Add some sparks with blue color for icy effect
            var emitter = particleService.CreateSparks(position, ownerId, 8);
            // Note: You could modify the sparks to be blue by accessing emitter.Config after creation
        }

        /// <summary>
        /// Example: Create a magical power-up effect
        /// </summary>
        public static ParticleEmitter CreatePowerUpEffect(IParticleService particleService, GameObject powerUp)
        {
            var emitter = particleService.CreateMagicalEffect(powerUp.Center, powerUp.Id, Color.Purple);
            
            // Note: Update the emitter position in the power-up's Update method if it moves
            
            return emitter;
        }

        /// <summary>
        /// Example: Create a custom particle effect
        /// This shows how to create completely custom particle behavior
        /// </summary>
        public static ParticleEmitter CreateCustomFireworks(IParticleService particleService, Vector2 position, Guid ownerId)
        {
            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 100f,
                MaxParticles = 40,
                EmissionRadius = 2f,
                ParticleLifetime = 2f,
                ParticleLifetimeVariation = 0.5f,
                ParticleSpeed = new Vector2(150f, 150f),
                ParticleSpeedVariation = new Vector2(75f, 75f),
                ParticleStartSize = 4f,
                ParticleEndSize = 0.5f,
                ParticleSizeVariation = 2f,
                ParticleStartColor = Color.Red,
                ParticleEndColor = Color.Yellow,
                Duration = 0.5f, // Short burst
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                UseGravity = true,
                GravityStrength = 100f,
                ParticleDrag = 1f
            };

            return particleService.CreateCustomEmitter(position, ownerId, config);
        }

        /// <summary>
        /// Example of how to integrate particle service into your main Game class
        /// Add this to your Game class constructor or initialization
        /// </summary>
        public static void IntegrateIntoGame(Game game, IObjectService objectService)
        {
            // Create the particle service
            var particleService = new ParticleService(objectService);
            
            // You can register it in your DI container or store it as a field
            // Registry.Register<IParticleService>(particleService);
            
            // In your game's Update method, you should call:
            // particleService.UpdateParticleEmitters(this);
            
            // Example usage in collision detection:
            /*
            if (projectileHitsTarget)
            {
                CreateProjectileExplosion(particleService, collision.Position, projectile.Owner);
            }
            
            if (iceProjectileHits)
            {
                CreateIceShatterEffect(particleService, collision.Position, projectile.Owner);
            }
            */
        }

        /// <summary>
        /// Example: Add fire trail effects to a player ship
        /// This shows how to create dynamic ship trails that respond to movement
        /// </summary>
        public static void AddFireTrailsToPlayer(IParticleService particleService, Player player)
        {
            // Create fire trail emitters for the player
            Vector2 trailPosition = new Vector2(
                player.Center.X + (player.IsPlayer1 ? -player.Rect.Width * 0.4f : player.Rect.Width * 0.4f),
                player.Center.Y
            );
            
            var fireTrail = particleService.CreateFireTrail(trailPosition, player.Id, player.IsPlayer1);
            var intenseFireTrail = particleService.CreateIntenseFireTrail(trailPosition, player.Id, player.IsPlayer1);
            
            // Start with trails disabled
            fireTrail.StopEmission();
            intenseFireTrail.StopEmission();
            
            // Note: You'll need to update these in your player's Update method:
            /*
            // In Player Update method:
            bool isMoving = Math.Abs(player.Speed.Y) > 50f;
            float movementIntensity = Math.Min(Math.Abs(player.Speed.Y) / 1200f, 1f);
            
            if (isMoving)
            {
                Vector2 newTrailPos = new Vector2(
                    player.Center.X + (player.IsPlayer1 ? -player.Rect.Width * 0.4f : player.Rect.Width * 0.4f),
                    player.Center.Y
                );
                
                if (movementIntensity > 0.7f)
                {
                    fireTrail.StopEmission();
                    intenseFireTrail.MoveTo(newTrailPos.X, newTrailPos.Y);
                    intenseFireTrail.StartEmission();
                }
                else
                {
                    intenseFireTrail.StopEmission();
                    fireTrail.MoveTo(newTrailPos.X, newTrailPos.Y);
                    fireTrail.StartEmission();
                }
            }
            else
            {
                fireTrail.StopEmission();
                intenseFireTrail.StopEmission();
            }
            */
        }

        /// <summary>
        /// Example: Create burst trail effect when player changes direction rapidly
        /// </summary>
        public static void CreateDirectionChangeEffect(IParticleService particleService, Player player)
        {
            // Create a burst of particles when direction changes
            Vector2 burstPosition = new Vector2(
                player.Center.X,
                player.Center.Y + player.Rect.Height * 0.3f
            );
            
            var config = new ParticleEmitterConfig
            {
                Shape = EmissionShape.Circle,
                EmissionRate = 200f,
                MaxParticles = 15,
                EmissionRadius = 8f,
                ParticleLifetime = 0.6f,
                ParticleLifetimeVariation = 0.2f,
                ParticleSpeed = new Vector2(80f, 80f),
                ParticleSpeedVariation = new Vector2(40f, 40f),
                ParticleStartSize = 4f,
                ParticleEndSize = 1f,
                ParticleSizeVariation = 2f,
                ParticleStartColor = player.IsPlayer1 ? Color.SkyBlue : Color.Orange,
                ParticleEndColor = player.IsPlayer1 ? Color.Blue : Color.Red,
                Duration = 0.2f,
                Loop = false,
                EmitOnStart = true,
                AutoDestroy = true,
                ParticleDrag = 2f
            };
            
            particleService.CreateCustomEmitter(burstPosition, player.Id, config);
        }
    }

    /// <summary>
    /// Example of how you might extend an existing projectile to include particle effects
    /// </summary>
    public class ParticleProjectileExample : GameObject
    {
        private readonly IParticleService _particleService;
        private ParticleEmitter _trailEmitter;

        public ParticleProjectileExample(Guid owner, SpriteWrapper sprite, Vector2 initialPosition, 
                                       Vector2 initialSize, Vector2 initialSpeed, IParticleService particleService) 
            : base(owner, sprite, initialPosition, initialSize, initialSpeed)
        {
            _particleService = particleService;
            
            // Create a trail effect when the projectile is created
            _trailEmitter = _particleService.CreateProjectileTrail(Center, Id, Color.Yellow);
        }

        public override void Update(Game game)
        {
            // Update projectile position
            float frameTime = Raylib.GetFrameTime();
            Move(Speed.X * frameTime, Speed.Y * frameTime);

            // Update trail emitter position to follow the projectile
            if (_trailEmitter != null && _trailEmitter.IsActive)
            {
                _trailEmitter.MoveTo(Center.X, Center.Y);
            }

            // Check if projectile is out of bounds
            if (Rect.X < 0 || Rect.X > Raylib.GetScreenWidth() || 
                Rect.Y < 0 || Rect.Y > Raylib.GetScreenHeight())
            {
                IsActive = false;
                
                // Stop the trail when projectile dies
                if (_trailEmitter != null)
                {
                    _trailEmitter.StopEmission();
                }
            }
        }

        public override void Draw()
        {
            Sprite?.Draw(Center, Rotation, Rect.Width, Rect.Height, Color);
        }

        // Call this when the projectile hits something
        public void OnHit(Vector2 hitPosition)
        {
            // Create explosion effect
            _particleService.CreateExplosion(hitPosition, Owner, Color.Orange, 1f);
            
            // Stop the trail
            if (_trailEmitter != null)
            {
                _trailEmitter.StopEmission();
            }
            
            IsActive = false;
        }
    }
}