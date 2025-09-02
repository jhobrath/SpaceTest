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
        private const float WindowSeconds = 3f;
        private const float MaxCount = 12f;

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
            modifiers.Projectile.OnShoot = projectile => HandleShotFired(projectile, modifiers);
            _playerModifiers = modifiers;
        }

        private void HandleShotFired(Projectile projectile, EffectModifiers modifiers)
        {
            double now = Raylib.GetTime();
            _shotTimestamps.Add(now);
            _shotTimestamps = _shotTimestamps.Where(t => now - t <= WindowSeconds).ToList();
            UpdateMultipliers(modifiers.Projectile);
            base.HandleShotFired(projectile);
        }

        private void UpdateMultipliers(PlayerProjectile projectile)
        {
            double now = Raylib.GetTime();
            _shotTimestamps = _shotTimestamps.Where(t => now - t <= WindowSeconds).ToList();
            int count = _shotTimestamps.Count;

            float maxMultiplier = 1.5f;
            float minMultiplier = 0.75f;
            float newMultiplier = maxMultiplier - ((maxMultiplier - minMultiplier) * Math.Min(count, MaxCount) / MaxCount);

            // Undo previous multiplier, apply new one
            projectile.SpeedMultiplier /= _currentMultiplier;
            projectile.SpeedMultiplier *= newMultiplier;

            if (_playerModifiers?.Stats != null)
            {
                //Fire rate inversely affected by mutliplier
                _playerModifiers.Stats.FireRateMultiplier *= (float)Math.Pow(_currentMultiplier, 2);
                _playerModifiers.Stats.FireRateMultiplier /= (float)Math.Pow(newMultiplier, 2);
            }

            _currentMultiplier = newMultiplier;
        }

        protected override void HandleShotFired(Projectile projectile)
        {
            // Not used anymore, logic moved to overload
        }
    }
}
