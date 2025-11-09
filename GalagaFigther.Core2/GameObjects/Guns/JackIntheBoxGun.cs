using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Guns
{
    public class JackIntheBoxGun : Gun
    {
        private List<GunBarrel> _barrels => [
            new GunBarrel(new(-1, 0), new(-1, -40)),
        ];

        public override float FireRate => 2f;

        public JackIntheBoxGun(GameObject owner) 
            : base(owner, new StillImageSprite(""))
        {
        }

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            return new() { { _barrels[0], new JackInTheBoxProjectile(shooter.Id) } };
        }
    }
}
