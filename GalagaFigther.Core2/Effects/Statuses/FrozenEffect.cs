using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core2.Effects.Statuses
{
    public class FrozenEffect : PlayerEffect
    {
        private SpriteDecoration _frozenDecoration;

        public FrozenEffect()
        {
            _frozenDecoration = new SpriteDecoration(
                new StillImageSprite("Sprites/Ships/MainShipBody_Frozen.png"), 
                Vector2.Zero, 
                false);
        }

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Display.BlueAlpha *= 1.25f; 
            modifiers.Stats.SpeedMultiplier *= 0.5f; 
            modifiers.Stats.FireRate *= 1.25f;
            modifiers.Decorations[this] = _frozenDecoration;
            
            // Add particle emitter factory function
            modifiers.CreateParticleEmitters[this] = CreateIceParticles;
        }

        private List<ParticleEmitter> CreateIceParticles(GameObject owner)
        {
            var iceConfig = ParticleEffectTemplates.Get("IceTrail");
            var emitter = new ParticleEmitter(owner.Id, new(84, 84), 30f) { Config = iceConfig };
            return [emitter];
            //var emitter1 = new ParticleEmitter(owner.Id, new(12,129), 30f) { Config = iceConfig };
            //var emitter2 = new ParticleEmitter(owner.Id, new(168-12, 129), 30f) { Config = iceConfig };
            //return [emitter1,emitter2];
        }
    }
}
