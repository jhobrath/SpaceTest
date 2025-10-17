using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Helpers
{
    public static class MathExtensions
    {
        public static Vector2 Max(params Vector2[] vector2s)
        {
            return new Vector2(
                vector2s.Select(x => x.X).Max(),
                vector2s.Select(x => x.Y).Max()
            );
        }

        public static Vector2 Min(params Vector2[] vector2s)
        {
            return new Vector2(
                vector2s.Select(x => x.X).Min(),
                vector2s.Select(x => x.Y).Min()
            );
        }

        public static Vector2 Clamp(Vector2 value, Vector2 min, Vector2 max)
        {
            value = Max(value, min);
            value = Min(value, max);
            return value;
        }
    }
}
