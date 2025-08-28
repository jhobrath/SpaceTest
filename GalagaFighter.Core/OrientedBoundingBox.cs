using System.Numerics;

namespace GalagaFighter.Core
{
    public class OrientedBoundingBox
    {
        public Vector2 Center { get; set; }
        public Vector2[] Corners { get; set; }

        public OrientedBoundingBox(Vector2 center, Vector2[] corners)
        {
            Center = center;
            Corners = corners;
        }
    }
}
