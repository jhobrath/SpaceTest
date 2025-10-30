using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using Raylib_cs;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core2.Services
{
    public static class ParticleEffectTemplates
    {
        private static readonly Dictionary<string, Func<ParticleEffectConfig>> _templates = [];

        static ParticleEffectTemplates()
        {
            InitializeTemplates();
        }

        public static ParticleEffectConfig Get(string name)
        {
            return _templates.TryGetValue(name, out var template) ? template() : new ParticleEffectConfig(name);
        }

        internal static void Reinitialize()
        {
            InitializeTemplates();
        }

        private static void InitializeTemplates()
        {
            _templates["EngineTrail"] = () => new ParticleEffectConfig("EngineTrail")
            {
                EmissionRate = 30f,
                StartSize = 8f,
                EndSize = 3f,
                Speed = new Vector2(0f, 200f),
                SpeedVariation = new(80f, 150f),
                Lifetime = 1.2f,
                Drag = 2f,
                Textures = ["dot_1", "dot_2", "dot_3", "dot_4", "dot_5"],
                Loop = true,
                Duration = -1f,
                StartColor = Color.Orange,
                EndColor = Color.Orange.ApplyAlpha(0),
                SizeVariation = 5f,
                EmissionRadius = 10f,
                ColorVariation = 50f
            };

            _templates["SmokeTrail"] = () => new ParticleEffectConfig("SmokeTrail")
            {
                EmissionRate = 15f,
                StartSize = 8f,
                EndSize = 10f,
                Speed = new Vector2(0f, 200f),
                SpeedVariation = new(80f, 25f),
                Lifetime = 1.5f,
                Drag = 2f,
                Textures = ["smoke_1", "smoke_2", "smoke_3", "smoke_4", "smoke_5"],
                Loop = true,
                Duration = -1f,
                StartColor = Color.Gray,
                EndColor = Color.Gray.ApplyAlpha(0),
                SizeVariation = 50f,
                EmissionRadius = 10f
            };

            _templates["SnowAura"] = () => new ParticleEffectConfig("SnowAura")
            {
                EmissionRadius = 50f,
                EmissionRate = 10f,
                StartSize = 6f,
                EndSize = 15f,
                Speed = new Vector2(0f,0f),
                SpeedVariation = new (50f,50f),
                SizeVariation = 50f,
                Lifetime = 1.0f,
                Drag = 1.5f,
                Textures = ["star_4", "star_5"],
                Loop = true,
                Duration = -1f,
                StartColor = Color.White.ApplyAlpha(.5f),
                EndColor = Color.Blue.ApplyAlpha(0),
                ColorVariation = 5f
            };

            _templates["SnowTrail"] = () => new ParticleEffectConfig("SnowTrail")
            {
                EmissionRadius = 20f,
                EmissionRate = 50f,
                StartSize = 6f,
                EndSize = 15f,
                Speed = new Vector2(0f, 0f),
                SpeedVariation = new(50f, 50f),
                SizeVariation = 20f,
                Lifetime = .25f,
                Drag = 1.5f,
                Textures = ["dot_2", "dot_1", "star_4", "star_5"],
                Loop = true,
                Duration = -1f,
                StartColor = Color.White.ApplyAlpha(.5f),
                EndColor = Color.Blue.ApplyAlpha(0),
                ColorVariation = 5f
            };

            _templates["SmokeTrail1"] = () => new ParticleEffectConfig("SmokeTrail1")
            {
                EmissionRate = 15f,
                StartSize = 8f,
                EndSize = 10f,
                Speed = new Vector2(0f, 200f),
                SpeedVariation = new(80f, 25f),
                Lifetime = 1.5f,
                Drag = 2f,
                Textures = ["smoke_1", "smoke_2", "smoke_3", "smoke_4", "smoke_5"],
                Loop = true,
                Duration = -1f,
                StartColor = Color.Gray,
                EndColor = Color.Gray.ApplyAlpha(0),
                SizeVariation = 50f,
                EmissionRadius = 10f
            };
        }
    }
}