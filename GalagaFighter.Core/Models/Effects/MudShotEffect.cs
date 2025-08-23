using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects
{
    public class MudShotEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/mudshot.png";
        public override bool IsProjectile => true;
        private readonly SpriteWrapper _sprite;
        protected override float Duration => 5f;

        private SpriteDecorations _decorations;

        public MudShotEffect()
        {
            _sprite = new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud.png"));
            _decorations = new SpriteDecorations
            {
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud_ShootRight.png"), 3, .6f)),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud_ShootLeft.png"), 3, .6f)),
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud_ShootBoth.png"), 3, .6f)),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipMud_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.Projectiles.Add(CreateProjectile);
            modifiers.Projectile.CollideDistanceFromEdge = 200f; // Hit screen edges at 200 pixels from edge
            modifiers.Projectile.OnCollide = CreateMudSplatEffect;
            modifiers.Projectile.IgnoreShipMovement = true;
            modifiers.Decorations = _decorations;
        }

        private Projectile CreateProjectile(IProjectileController controller, Player owner, Vector2 position, PlayerProjectile modifiers)
            => new MudProjectile(controller, owner, position, modifiers);

        private List<GameObject> CreateMudSplatEffect(Player player, Projectile Projectile)
        {
            var topLeft = new Vector2(Projectile.Center.X - 150f, Projectile.Center.Y - 150f);
            var mudSplat = new MudSplat(player.Owner, topLeft, new Vector2(300f, 300f));
            var mudSplatEffect = new MudSplatEffect(player, mudSplat);
            player.Effects.Add(mudSplatEffect);
            return [mudSplat];
        }
    }
}