using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class NinjaShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/ninjashot.png";
        public override bool IsProjectile => true;
        protected override float Duration => 10f;

        private readonly SpriteDecorations _decorations;

        public NinjaShotEffect()
        {
            _decorations = new SpriteDecorations
            {
                Guns = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipNinjaGuns.png"))),
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipNinjaGuns_ShootBoth.png"))),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipNinjaGuns_ShootLeft.png"))),
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipNinjaGuns_ShootRight.png"))),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Projectile.OnShootProjectiles.Add(CreateProjectile);
            modifiers.Stats.FireRateMultiplier *= .45f;
            modifiers.Projectile.RotationOffset += 10f;
            modifiers.Projectile.VerticalPositionIncrement = -150 + 300f * (float)Game.Random.NextDouble();
            modifiers.Projectile.OnClone = (projMods) => projMods.VerticalPositionIncrement = -150 + 300f * (float)Game.Random.NextDouble();
            modifiers.Decorations = _decorations;
        }

        private Projectile CreateProjectile(IProjectileController controller, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new NinjaProjectile(controller, owner, position, modifiers);
    }
}
