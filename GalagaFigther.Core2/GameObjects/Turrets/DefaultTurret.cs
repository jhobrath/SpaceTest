using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
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

        private readonly List<TurretGun> _guns;
        public override List<TurretGun> Guns => _guns;

        public DefaultTurret(Guid owner, Vector2 position) 
            : base(owner, position, Vector2.One*80f, Vector2.Zero, _sprite)
        {
            _guns = [new DefaultTurretGun(this)];
        }
    }

    public class DefaultTurretGun : TurretGun
    {
        private static readonly StillImageSprite _sprite = new("Sprites/Turrets/turret_barrels.png");

        public override List<Gun> Guns => [
            new Gun([
                new GunBarrel(new(0,0),new(40,0)),
                new GunBarrel(new(0,0),new(-40,0))
            ], HandleShoot)
        ];

        public DefaultTurretGun(DefaultTurret turret)
            : base(turret.Id, turret.Rect.Position, turret.Rect.Size, Vector2.Zero, _sprite)
        {
            AngularVelocity = 180f;
        }

        private List<GameObject> HandleShoot(Guid id)
        {
            return [new DefaultProjectile(id, Vector2.Zero)];
        }
    }
}
