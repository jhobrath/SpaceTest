using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects
{
    public class SpringAttachment : GameObject
    {
        public Spring? Spring { get; set; } = null;
        public float ReelSpeed { get; set; } = 0f;

        public SpringAttachment(Guid owner, Vector2 position)
            : base(owner, position, new(0, 0), new(0, 0), new StillImageSprite(""))
        {
        }
    }

    public class Spring
    {
        public SpringAttachment Start { get; set; }
        public SpringAttachment? End { get; set; }
        public float RestLength { get; } // Natural length of the spring
        public float Stiffness { get; } // for visual
        public List<SpringPoint> Points { get; } = [];
        public float CoilAmplitude { get; } = 12f; // Default amplitude for spring coil rendering


        //State
        public bool Initialized { get; set; } = false;
        public float CurrentLength { get; set; } // Current length, can be set by collision
        public bool IsCompressed { get; set; }
        public float ExpandRate { get; set; }


        public Spring(SpringAttachment start, float restLength, float stiffness, float damping, SpringAttachment? end = null)
        {
            Start = start;
            End = end;
            RestLength = restLength;
            CurrentLength = restLength; // Initialize to rest length
            Stiffness = stiffness;

            // For visualization or simulation, you can subdivide the spring into points
            int pointCount = Math.Max(2, Convert.ToInt32(restLength / 5f));
            Points = Enumerable.Range(0, pointCount).Select(x => new SpringPoint()).ToList();
        }
    }

    public class SpringPoint
    {
        public Vector2 CurrentPosition { get; set; }
        public Vector2 OldPosition { get; set; }
        public float DistanceFromAttachment { get; set; } // New: distance along the spring direction
    }
}
