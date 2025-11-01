using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Guns
{
    public class ShotGun : Gun
    {
        private List<GunBarrel> _barrels = new()
        {
            // Rotated -90 degrees: (x, y) becomes (y, -x)
            new GunBarrel(new(0, 0), new(0, -425))
        };

        public override float FireRate => 1f;

        public override List<GunBarrel> Barrels => _barrels;

        public ShotGun(GameObject owner) 
            : base(owner, new StillImageSprite(""))
        {
        }

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            return new()
            {
                { _barrels[0], new ShotGunShellProjectile(shooter.Id) { Palette = shooter.Palette }  }
            };
        }
    }
}
