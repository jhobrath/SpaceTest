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

        public DefaultTurret(Guid owner, Vector2 position) 
            : base(owner, position, Vector2.One*80f, Vector2.Zero, _sprite)
        {
        }
    }

    public class DefaultTurretGun : TurretGun
    {
        private static readonly StillImageSprite _sprite = new("Sprites/Turrets/turret_barrels.png");

        public override List<TurretGunOffset> GunOffsets => 
        [
            new TurretGunOffset {
                Position = new(40,0),
                SpeedMultiplier = Vector2.One
            },
            new TurretGunOffset {
                Position = new(-40,0),
                SpeedMultiplier = new(-1,-1)
            }
        ];

        public override Func<Guid, List<GameObject>> OnShoot => (Guid id) =>
            [new DefaultProjectile(id, Vector2.Zero)];

        public DefaultTurretGun(DefaultTurret turret)
            : base(turret.Id, turret.Rect.Position, turret.Rect.Size, Vector2.Zero, _sprite)
        {
            AngularVelocity = 180f;
        }
    }
}
