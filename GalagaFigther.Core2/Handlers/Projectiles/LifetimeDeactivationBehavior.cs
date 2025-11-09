using GalagaFighter.Core2.GameObjects.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public class LifetimeDeactivationBehavior : ProjectileBehaviorBase
    {
        public override void Update(Projectile projectile, float frameTime)
        {
            if (projectile.Lifetime != -1f)
            {
                projectile.Lifetime -= frameTime;
                if (projectile.Lifetime < 0f)
                {
                    projectile.IsActive = false;
                }
            }
        }
    }
}
