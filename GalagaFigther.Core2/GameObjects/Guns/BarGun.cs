using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Guns
{
    public class BarGun : Gun
    {
        private List<GunBarrel> _barrels => [
            new GunBarrel(new(0, 0),new(56,0)),
        ];

        public override List<GunBarrel> Barrels => _barrels;
        public override float FireRate => .65f;

        public BarGun(GameObject owner)
            : base(owner, new StillImageSprite("Sprites/Ships/BarGun.png"))
        {
        }

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            return new()
            {
                {
                    _barrels[0],
                    new BarProjectile(shooter.Id) { Palette = shooter.Palette }
                }
            };
        }
    }
}
