using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;

namespace GalagaFighter.Core.Models.Effects.Projectiles
{
    public class DefaultShootEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/firerate1.png";
        public override bool IsProjectile => true;
        private SpriteDecorations _decorations;
        private Color? _palleteSwap = null;
        private List<double> _shotTimestamps = new();
        private const float BulletSpeedWindow = 3f;
        private const float BulletSpeedMaxAffectiveCount = 12f;

        public DefaultShootEffect()
        {
            _decorations = SetDecorations();
        }

        private EffectModifiers _playerModifiers;
        private float _currentMultiplier = 1f;

        private SpriteDecorations SetDecorations()
        {
            return new SpriteDecorations()
            {
                ShootBoth = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_ShootBoth.png"))),
                ShootLeft = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_ShootLeft.png"))),
                ShootRight = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_ShootRight.png"))),
                Move = new SpriteDecoration(new SpriteWrapper(TextureService.Get("Sprites/Ships/MainShip_Move.png")))
            };
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Decorations = _decorations; 
            modifiers.Projectile.OnShootProjectiles.Add((updater, owner, position, modifiers) => new DefaultProjectile(updater, owner, position, modifiers, owner.PalleteSwap));
            modifiers.Projectile.Homing += .5f;
            modifiers.Stats.FireRateMultiplier *= 1/1.5f;
            _playerModifiers = modifiers;
        }
    }
}
