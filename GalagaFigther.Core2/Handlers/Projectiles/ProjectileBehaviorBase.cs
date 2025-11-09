using GalagaFighter.Core2.GameObjects.Projectiles;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public abstract class ProjectileBehaviorBase : IProjectileBehavior
    {
        public virtual void Update(Projectile projectile, float frameTime) { }
        public virtual void OnCreate(Projectile projectile) { }
        public virtual void OnDestroy(Projectile projectile) { }
    }
}
