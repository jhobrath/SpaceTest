using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Projectiles;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public class RotationFollowsSpeedBehavior : ProjectileBehaviorBase
    {
        public override void Update(Projectile projectile, float frameTime)
        {
            if (projectile.AngularVelocity != 0)
                return;

            var theta = MathF.Atan2(projectile.Speed.Y, projectile.Speed.X);
            var rotation = theta * 180.0f / MathF.PI;
            projectile.Rotation = rotation;
        }
    }
}
