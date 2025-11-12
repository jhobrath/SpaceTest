using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Guns
{
    public class PowerShotGun : Gun
    {
        public PowerShotGun(GameObject owner) 
            : base(owner, new StillImageSprite(""))
        {
            RecoveryTime = .75f;
        }

        private List<GunBarrel> _barrels => [
            // Rotated -90 degrees: (x, y) becomes (y, -x)
            new GunBarrel(new(-1, 0), new(-1, -56)),
        ];

        public override List<GunBarrel> Barrels => _barrels;
        public override float FireRate => 0f;
        public override bool IsBuildUp => true;

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            return new() { { _barrels[0], new PowerShotProjectile(shooter.Id) } };
        }
    }
}
