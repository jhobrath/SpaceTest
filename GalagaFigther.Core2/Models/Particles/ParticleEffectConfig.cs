using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Particles;
using Raylib_cs;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Models.Particles
{
    public class ParticleEffectConfig : IGameObjectData<ParticleEmitter>
    {
        public string Name { get; set; }
        public float EmissionRate { get; set; }
        public float EmissionRadius { get; set; }
        public float Lifetime { get; set; }
        public float Drag { get; set; }
        public string[] Textures { get; set; }
        public bool Loop { get; set; }
        public float Duration { get; set; }

        public Color StartColor { get; set; }
        public Color EndColor { get; set; }

        public float StartSize { get; set; }
        public float EndSize { get; set; }
        public float SizeVariation { get; set; }

        public Vector2 Speed { get; set; }
        public Vector2 SpeedVariation { get; set; }
        public float ColorVariation { get; set; }

        public ParticleEffectConfig()
        {
            Name = "Default";
            EmissionRate = 20f;
            Lifetime = 1f;
            Drag = 1f;
            Textures = ["dot"];
            StartSize = 10f;
            EndSize = 4f;
            StartColor = Color.White;
            EndColor = Color.White.ApplyAlpha(0);
            Loop = true;
            Duration = -1f;
            Speed = Vector2.One*100f;
        }
        
        public ParticleEffectConfig(string name) : this()
        {
            Name = name;
        }
    }
}