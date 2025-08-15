using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Models
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
