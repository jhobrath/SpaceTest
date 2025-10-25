using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Particles;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Models.Particles
{
    public class ParticleEffectConfig : IGameObjectData<ParticleEmitter>
    {
        public string Name { get; set; }
        public string AttachmentPoint { get; set; }
        public float BaseEmissionRate { get; set; }
        public float BaseSize { get; set; }
        public float BaseSpeed { get; set; }
        public float BaseLifetime { get; set; }
        public float BaseDrag { get; set; }
        public string[] BaseTextures { get; set; }
        public bool Loop { get; set; }
        public float Duration { get; set; }
        public bool FollowRotation { get; set; }
        
        public ParticleEffectConfig()
        {
            Name = "Default";
            AttachmentPoint = "Center";
            BaseEmissionRate = 20f;
            BaseSize = 10f;
            BaseSpeed = 100f;
            BaseLifetime = 1f;
            BaseDrag = 1f;
            BaseTextures = ["particle"];
            Loop = true;
            Duration = -1f;
            FollowRotation = false;
        }
        
        public ParticleEffectConfig(string name) : this()
        {
            Name = name;
        }
    }
}