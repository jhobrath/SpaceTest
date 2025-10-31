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
    public class DefaultGun : Gun
    {
        private List<GunBarrel> _barrels => [
            new GunBarrel(new(-46, 0), new(-46, -30)),
            new GunBarrel(new(46, 0), new(46, -30))
        ];

        private int _gunIndex = 0;
        private readonly int _offset;

        public override List<GunBarrel> Barrels => _barrels;
        public override float FireRate => .3f;

        public DefaultGun(GameObject owner, int offset) 
            : base(owner, new StillImageSprite("Sprites/Ships/MainShipGuns.png"))
        {
            _offset = offset;
        }

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            _gunIndex = (_gunIndex + 1 + _offset) % 2;
            
            return new() 
            { 
                { 
                    _barrels[_gunIndex], 
                    new DefaultProjectile(shooter.Id) { Palette = shooter.Palette } 
                } 
            };
        }
    }
}
