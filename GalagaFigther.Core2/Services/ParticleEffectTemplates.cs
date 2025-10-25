using GalagaFighter.Core2.Models.Particles;
using System.Collections.Generic;

namespace GalagaFighter.Core2.Services
{
    public static class ParticleEffectTemplates
    {
        private static readonly Dictionary<string, ParticleEffectConfig> _templates = [];

        static ParticleEffectTemplates()
        {
            InitializeTemplates();
        }

        public static ParticleEffectConfig Get(string name)
        {
            return _templates.TryGetValue(name, out var template) ? template : new ParticleEffectConfig(name);
        }

        private static void InitializeTemplates()
        {
            _templates["EngineTrail"] = new ParticleEffectConfig("EngineTrail")
            {
                AttachmentPoint = "Engine",
                BaseEmissionRate = 30f,
                BaseSize = 8f,
                BaseSpeed = 50f,
                BaseLifetime = 1.2f,
                BaseDrag = 2f,
                BaseTextures = ["fire_1", "fire_2", "fire_3"],
                Loop = true,
                Duration = -1f,
                FollowRotation = true
            };

            _templates["MuzzleFlash"] = new ParticleEffectConfig("MuzzleFlash")
            {
                AttachmentPoint = "Gun",
                BaseEmissionRate = 100f,
                BaseSize = 12f,
                BaseSpeed = 200f,
                BaseLifetime = 0.3f,
                BaseDrag = 5f,
                BaseTextures = ["spark_4", "spark_5"],
                Loop = false,
                Duration = 0.1f,
                FollowRotation = false
            };

            _templates["Explosion"] = new ParticleEffectConfig("Explosion")
            {
                AttachmentPoint = "Center",
                BaseEmissionRate = 200f,
                BaseSize = 15f,
                BaseSpeed = 300f,
                BaseLifetime = 1.5f,
                BaseDrag = 3f,
                BaseTextures = ["fire_4", "fire_5", "spark_3", "spark_4"],
                Loop = false,
                Duration = 0.5f,
                FollowRotation = false
            };

            _templates["IceTrail"] = new ParticleEffectConfig("IceTrail")
            {
                AttachmentPoint = "Engine",
                BaseEmissionRate = 25f,
                BaseSize = 6f,
                BaseSpeed = 30f,
                BaseLifetime = 1.0f,
                BaseDrag = 1.5f,
                BaseTextures = ["star_4", "star_5"],
                Loop = true,
                Duration = -1f,
                FollowRotation = true
            };

            _templates["Smoke"] = new ParticleEffectConfig("Smoke")
            {
                AttachmentPoint = "Center",
                BaseEmissionRate = 15f,
                BaseSize = 20f,
                BaseSpeed = 40f,
                BaseLifetime = 3f,
                BaseDrag = 0.5f,
                BaseTextures = ["smoke_1", "smoke_2", "smoke_3"],
                Loop = true,
                Duration = -1f,
                FollowRotation = false
            };
        }
    }
}