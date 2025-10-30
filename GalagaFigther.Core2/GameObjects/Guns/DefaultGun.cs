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
            new GunBarrel(new(0, -46),new(30,-46)),
            new GunBarrel(new(0, 46),new(30,46))
        ];

        private int _gunIndex = 0;
        private int _shotCount = 0;

        public override List<GunBarrel> Barrels => _barrels;

        public DefaultGun(GameObject owner) 
            : base(owner, new StillImageSprite("Sprites/Ships/MainShipGuns.png"))
        {
        }

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            _gunIndex = (_gunIndex + 1) % 2;
            _shotCount++;
            
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
