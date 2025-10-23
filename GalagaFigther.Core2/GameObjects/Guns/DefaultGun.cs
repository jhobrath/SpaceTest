using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
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
        private static List<GunBarrel> _barrels => [
            new GunBarrel(new(0, -46),new(30,-46)),
            new GunBarrel(new(0, 46),new(30,46))
        ];

        private int _gunIndex = 0;

        public override List<GunBarrel> Barrels => _barrels;
        private readonly static StillImageSprite _sprite = new("Sprites/Ships/MainShipGuns.png");

        public DefaultGun(GameObject owner) 
            : base(owner, _sprite)
        {
        }

        public override Dictionary<GunBarrel, GameObject> Shoot(Guid owner)
        {
            _gunIndex = (_gunIndex + 1) % 2;
            return new() { { _barrels[_gunIndex], new DefaultProjectile(owner, Vector2.Zero) } };
        }
    }
}
