using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Models.Projectiles;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Handlers.Projectiles;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public abstract class Projectile : GameObject
    {
        public float Lifetime { get; set; } = -1f;
        public virtual List<ParticleEffectConfig> EmitterConfigurations => [];

        public Projectile(Guid owner, Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite)
            : base(owner, position, size, speed, sprite)
        {
            Owner = owner;
        }

        public bool IsTransformChild { get; set; } = false;
        public float Damage { get; set; } = 0f;
        public float Homing { get; set; } = 0f;
        public float Veer { get; set; } = 0f;
        public bool DestroyProjectiles { get; set;  } = false;
        public bool DestroyOnHit { get; set; } = false;
        public bool Collidable { get; set; } = true;
        public bool SticksToWall { get; set; } = false;

        public virtual List<PlayerEffect> CreateEffects(Player player) => [];

        // Changed: Two lists for behavior types and state models
        public List<Type> Behaviors { get; } = new();
        public List<IGameObjectData<Projectile>> StateModels { get; } = new();

        // New properties for explicit collision response assignment
        public Type? PlayerCollisionHandler { get; set; } = typeof(DefaultProjectilePlayerCollisionBehavior);
        public Type? EdgeCollisionHandler { get; set; } = typeof(DefaultProjectileEdgeCollisionBehavior);
    }

    public interface IProjectileBehavior
    {
        void Update(Projectile projectile, float frameTime);
        void OnCreate(Projectile projectile);
        void OnDestroy(Projectile projectile);
    }
}
