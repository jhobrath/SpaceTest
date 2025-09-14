using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class ElectrocutedEffect : StatusEffect
    {
        private EffectModifiers? _modifiers;
        private ParticleEffect _particleEffect;
        private SpriteDecoration _electrocutedDecoration;

        public override string IconPath => "Sprites/Effects/burning.png";
        public override int MaxCount => 2;
        protected override float Duration => 1f;

        public ElectrocutedEffect()
        {
            _particleEffect = ParticleEffectsLibrary.Get("LightningChain");
            _particleEffect.ParticleStartSize = 15f;
            _particleEffect.MaxParticles = 50;
            _particleEffect.Offset = Vector2.One * _particleEffect.ParticleStartSize/2;
            _particleEffect.FollowRotation = true;
            _particleEffect.EmissionRate = 50f;
            _particleEffect.ParticleStartColor = Color.Gold.ApplyAlpha(.5f);
            _particleEffect.ParticleEndColor = Color.DarkBlue.ApplyAlpha(.8f);// Fast emission for continuous effect

            _electrocutedDecoration = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipBody_Electrocuted.png"), Vector2.Zero, new Vector2(160, 160));
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.SpeedMultiplier *= 0f;
            modifiers.Display.RedAlpha *= .9f;
            modifiers.Display.GreenAlpha *= .8f;
            modifiers.Display.BlueAlpha *= .9f;
            modifiers.Jiggle = 1f;

            modifiers.ParticleEffects.Add(_particleEffect);

            modifiers.Decorations["Electrocuted"] = _electrocutedDecoration;

            _modifiers = modifiers;
        }

        public override void OnUpdate(float frameTime)
        {
            var opacity = frameTime % 1f < .25f
                ? 1f
                : 0f;

            _electrocutedDecoration.Sprite.Color = Color.White.ApplyAlpha(opacity);

            base.OnUpdate(frameTime);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }
    }
}
