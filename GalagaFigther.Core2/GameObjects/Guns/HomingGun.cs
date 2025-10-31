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
    public class HomingGun : Gun
    {
        private List<GunBarrel> _barrels => [
            new GunBarrel(new(0, 0), new(-46, 0)),
            new GunBarrel(new(0, 0), new(46, 0))
        ];

        private int _gunIndex = 0;
        private int _shotCount = 0;

        public override List<GunBarrel> Barrels => _barrels;
        public override float FireRate => 1.5f;

        public HomingGun(GameObject owner) 
            : base(owner, new StillImageSprite("Sprites/Ships/SidewinderGuns.png"))
        {
        }

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            _gunIndex = (_gunIndex + 1) % 2;
            return new() 
            { 
                { 
                    _barrels[_gunIndex], 
                    new HomingProjectile(shooter.Id) { Palette = shooter.Palette } 
                }
            };
        }
    }
}
