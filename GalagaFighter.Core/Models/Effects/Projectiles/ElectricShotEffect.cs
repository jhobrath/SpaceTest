using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class ElectricShotEffect : PlayerEffect
    {
        private readonly SpriteWrapper _sprite;

        public override string IconPath => "Sprites/effects/plasmaball.png";
        public override bool IsProjectile =>  true;

        private readonly SpriteDecorations _decorations;

        public ElectricShotEffect(Color? color)
        {
            _sprite = new SpriteWrapper("Sprites/Ships/MainShip.png", color ?? Color.White);
            _decorations = new SpriteDecorations
            {
                Guns = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipElectricGuns.png"))),
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipElectricGuns_ShootBoth.png"), 3, .15f, repeat: false)),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipElectricGuns_ShootLeft.png"), 3, .15f, repeat: false)),
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShipElectricGuns_ShootRight.png"), 3, .15f, repeat: false)),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Sprite = _sprite;
            modifiers.Projectile.Phases.Add(this, [1.0f, 1.4f, 1.8f, 3f, 7f]);
            modifiers.Projectile.OnPhaseChange.Add(this, HandlePhaseChange);
            modifiers.Projectile.OnShootProjectiles.Add((controller, player, position, modifiers) =>
            {
                return new ElectricProjectile(controller, player, position, modifiers);
            });
            modifiers.Projectile.CollideDistanceFromPlayer = 135f;
            modifiers.Decorations = _decorations;
        }

        public override void OnUpdate(float frameTime)
        {
            base.OnUpdate(frameTime);
        }

        private void HandlePhaseChange(Projectile projectile, int phase)
        {
            if (phase == 5)
            {
                projectile.IsActive = false;
            }
        }
    }
}
