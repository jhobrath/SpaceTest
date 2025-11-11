using GalagaFighter.Core2.GameObjects.Guns;
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
using Raylib_cs;

namespace GalagaFighter.Core2.GameObjects.Turrets
{
    public class NinjaTurret : Turret
    {
        private readonly List<Gun> _guns;
        public override List<Gun> Guns => _guns;

        public NinjaTurret(GameObject owner)
            : base(owner.Id, Vector2.Zero, Vector2.One * 80f, Vector2.Zero, new StillImageSprite("Sprites/Turrets/NinjaTurret.png"))
        {
            Palette = owner.Palette;
            _guns = [new NinjaTurretGun(this)];
            WorldPosition = owner.WorldPosition;

            var deco = new SpriteDecoration(new StillImageSprite("Sprites/Turrets/NinjaTurret_spinner.png") { PaletteSwap = PaletteSwap.CreateSwap(Color.Red,Palette) })
            {
                AngularVelocity = 400f,
                Depth = 2
            };
            Decorations.Add(deco);
        }
    }

    public class NinjaTurretGun : Gun
    {
        public override List<GunBarrel> Barrels => _barrels;

        public override float FireRate => .5f;

        public static List<GunBarrel> _barrels => [
            new GunBarrel(new(0,0),new(0,-40)) { Recoil = -10f }
        ];

        public NinjaTurretGun(NinjaTurret turret)
            : base(turret, new StillImageSprite("Sprites/Turrets/NinjaTurret_barrel.png"))
        {
            Palette = turret.Palette;
            RotationHoming = 25f;
            MinRotation = -45f;
            MaxRotation = +45;
            IsPlayerGun = false;
        }

        public override Dictionary<GunBarrel, Projectile> Shoot(GameObject shooter)
        {
            return _barrels.ToDictionary(x => x, x => (Projectile)new NinjaProjectile(shooter.Id) { Palette = shooter.Palette });
        }
    }
}
