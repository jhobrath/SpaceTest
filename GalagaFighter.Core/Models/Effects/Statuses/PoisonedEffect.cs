using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class PoisonedEffect : StatusEffect
    {
        private SpriteDecorations _decorations;
        private ParticleEffect _particleEffect;

        public override int MaxCount => 1;

        public override string IconPath => "Sprites/Effects/Statuses/poisoned.png";

        public PoisonedEffect()
        {
            _decorations = new SpriteDecorations
            {
                { "Poisoned", new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipBody_Poisoned.png"), Vector2.Zero, new Vector2(160, 160)) }
            };

            _particleEffect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.Smoke);
            _particleEffect.ParticleStartColor = Color.Green.ApplyAlpha(.1f);
            _particleEffect.ParticleEndColor = Color.Purple.ApplyAlpha(0f);
            _particleEffect.ParticleStartSize = 100f;
            _particleEffect.ParticleEndSize = 160f;
            _particleEffect.Offset = -Vector2.One * (_particleEffect.ParticleStartSize / 2f);
            _particleEffect.EmissionRate = 25f;
            _particleEffect.EmissionRadius = 50f;
            _particleEffect.Shape = EmissionShape.Point;
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Decorations.Apply(_decorations);
            modifiers.ParticleEffects.Add(_particleEffect);
            modifiers.Projectile.OnShoot.Add(HurtPlayer);
        }

        public override void OnUpdate(float frameTime)
        {

            base.OnUpdate(frameTime);
        }

        private void HurtPlayer(Player player, Projectile projectile)
        {
            player.Health -= .25f;
        }
    }
}
