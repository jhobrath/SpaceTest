using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core.Models.Effects.Statuses
{
    public class FrozenEffect : StatusEffect
    {
        private SpriteDecoration _frozenDecoration;
        private ParticleEffect _effect;

        public override string IconPath => "Sprites/Effects/frozen.png";
        protected override float Duration => 5f;

        public FrozenEffect()
        {
            _frozenDecoration = new SpriteDecoration(new SpriteWrapper("Sprites/Ships/MainShipBody_Frozen.png"), Vector2.Zero, new Vector2(160,160)) {  FollowRotation = true };
            _effect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.FireTrail);
            _effect.Shape = EmissionShape.Circle;
            _effect.ParticleStartSize = 25f;
            _effect.ParticleEndSize = 50f;
            _effect.EmissionRadius = 60f;
            _effect.EmissionRate = 10f;
            _effect.MaxParticles = 15;
            _effect.ParticleStartColor = Color.SkyBlue;
            _effect.ParticleEndColor = Color.DarkBlue;
            _effect.ParticleSpeedVariation = 100f * Vector2.One;
            _effect.Offset = new Vector2(-12.5f, -12.5f);
        }

        public override void Apply(EffectModifiers modifiers)
        {
            modifiers.Stats.SpeedMultiplier *= .667f;
            modifiers.Stats.Shield *= 1.15f;
            modifiers.Stats.FireRateMultiplier *= 1.25f;

            modifiers.Jiggle = 1f;

            var frozenDecoKey = Game.Random.Next(1, 9);
            modifiers.Decorations["Frozen" + frozenDecoKey] = _frozenDecoration;

            //modifiers.ParticleEffects.Add(_effect);
            _modifiers = modifiers;
        }

        private float _lifeTime = 0f;
        private EffectModifiers _modifiers;

        public override void OnUpdate(float frameTime)
        {
            _lifeTime += frameTime;
            var opacity = (Duration - _lifeTime) / Duration;

            _frozenDecoration.Sprite.Color = Color.White.ApplyAlpha(opacity/2);

            _modifiers.Jiggle = opacity * 2f;

            base.OnUpdate(frameTime);
        }
    }
}
