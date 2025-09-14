using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class CharredEffect : PlayerEffect
    {
        public override string IconPath => "Sprites/Effects/Statuses/charred.png";

        private readonly ParticleEffect _smokeEffect;
        private readonly ParticleEffect _smokeRingEffect;

        private readonly SpriteDecoration _charDecoration;
        private readonly SpriteDecoration _cindersDecoration;
        private readonly SpriteDecoration _cinders2Decoration;

        protected override float Duration => 5f ;

        public CharredEffect()
        {
            _smokeEffect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.Smoke);
            _smokeRingEffect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.SmokeRings);

            _smokeEffect.EmissionRadius = 80f;
            _smokeRingEffect.EmissionRadius = 80f;

            _charDecoration = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipBody_Charred.png"), Vector2.Zero, new Vector2(160, 160));
            _cindersDecoration = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipBody_Cinders.png"), Vector2.Zero, new Vector2(160, 160));
            _cinders2Decoration = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipBody_Cinders2.png"), Vector2.Zero, new Vector2(160, 160));
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.ParticleEffects.Add(_smokeEffect);
            //modifiers.ParticleEffects.Add(_smokeRingEffect);

            modifiers.Decorations["Charring"] = _charDecoration;
            modifiers.Decorations["Cinders"] = _cindersDecoration;
            modifiers.Decorations["Cinders2"] = _cinders2Decoration;
        }

        private float _lifeTime = 0f;
        public override void OnUpdate(float frameTime)
        {
            _lifeTime += frameTime;
            var opacity = Math.Abs(1 - 2*(_lifeTime % 1f));

            _cindersDecoration.Sprite.Color = Color.White.ApplyAlpha(opacity);
            _cinders2Decoration.Sprite.Color = Color.White.ApplyAlpha(1-opacity);
            base.OnUpdate(frameTime);
        }
    }
}
