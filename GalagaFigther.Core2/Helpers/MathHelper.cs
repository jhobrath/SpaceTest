using System;
using System.Numerics;

namespace GalagaFighter.Core2.Helpers
{
    public static class MathHelper
    {
        public static float Lerp(float start, float end, float t)
        {
            return start + (end - start) * t;
        }
    }
}