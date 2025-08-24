using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Players
{
    public class PlayerDisplay
    {
        public Vector2 SizeMultiplier { get; set; } = new(1f,1f);
        public float RedAlpha { get; set; } = 1.0f;
        public float BlueAlpha { get; set; } = 1.0f;
        public float GreenAlpha { get; set; } = 1.0f;
        public float Opacity { get; set; } = 1f;
        public float RotationOffset { get; set; } = 0f;

        public float Rotation { get; set; } = 0f; //Always leave at zero

        public PlayerDisplay() 
        {
        }
    }
}
