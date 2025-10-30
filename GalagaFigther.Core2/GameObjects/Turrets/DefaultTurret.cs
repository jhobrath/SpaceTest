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
        private readonly List<Gun> _guns;
        public override List<Gun> Guns => _guns;

        public DefaultTurret(GameObject owner) 
            : base(owner.Id, Vector2.Zero, Vector2.One*80f, Vector2.Zero, new StillImageSprite("Sprites/Turrets/Turret.png"))
        {
            Palette = owner.Palette;
            _guns = [new DefaultTurretGun(this)];
            WorldPosition = owner.WorldPosition;
        }
    }

    public class DefaultTurretGun : Gun
    {
        public override List<GunBarrel> Barrels => _barrels;

        public static List<GunBarrel> _barrels => [
            new GunBarrel(new(0,0),new(40,0)),
            new GunBarrel(new(0,0),new(-40,0))
        ];

        public DefaultTurretGun(DefaultTurret turret)
            : base(turret, new StillImageSprite("Sprites/Turrets/turret_barrels.png"))
        {
            Palette = turret.Palette;
            AngularVelocity = 180f;
        }

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            return _barrels.ToDictionary(x => x, x => (Projectile)new DefaultProjectile(shooter.Id) { Palette = shooter.Palette });
        }
    }
}
