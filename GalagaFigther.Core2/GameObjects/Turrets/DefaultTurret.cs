using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Turrets
{
    public class DefaultTurret : Turret
    {
        private static readonly StillImageSprite _sprite = new("Sprites/Turrets/Turret.png");

        private readonly List<Gun> _guns;
        public override List<Gun> Guns => _guns;

        public DefaultTurret(Guid owner, Vector2 position) 
            : base(owner, position, Vector2.One*80f, Vector2.Zero, _sprite)
        {
            _guns = [new DefaultTurretGun(this)];
        }
    }

    public class DefaultTurretGun : Gun
    {
        private static readonly StillImageSprite _sprite = new("Sprites/Turrets/turret_barrels.png");
        public override List<GunBarrel> Barrels => _barrels;

        public static List<GunBarrel> _barrels => [
            new GunBarrel(new(0,0),new(40,0)),
            new GunBarrel(new(0,0),new(-40,0))
        ];

        public DefaultTurretGun(DefaultTurret turret)
            : base(turret, _sprite)
        {
            AngularVelocity = 180f;
        }

        public override Dictionary<GunBarrel, GameObject> Shoot(Guid id)
        {
            return _barrels.ToDictionary(x => x, x => (GameObject)new DefaultProjectile(id, Vector2.Zero));
        }
    }
}
