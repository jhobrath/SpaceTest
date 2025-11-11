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
            new GunBarrel(new(-48, 0), new(-48, -34)),
            new GunBarrel(new(43, 0), new(43, -34))
        ];

        private int _gunIndex = 0;
        public bool ShootBoth { get; set; }

        public override List<GunBarrel> Barrels => _barrels;
        public override float FireRate => ShootBoth ? .5f : .3f;

        public DefaultGun(GameObject owner, bool shootBoth) 
            : base(owner, new StillImageSprite("Sprites/Ships/MainShipGuns.png"))
        {
            ShootBoth = shootBoth;
            RecoveryTime = .15f;
        }

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            if(ShootBoth)
                return _barrels.ToDictionary(x => x, x => (Projectile)new DefaultProjectile(shooter.Id) { Palette = shooter.Palette });

            _gunIndex = (_gunIndex + 1) % 2;
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
