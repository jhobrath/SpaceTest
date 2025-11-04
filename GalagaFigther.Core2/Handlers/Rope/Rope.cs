using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Rope
{
    public class RopeAttachment : GameObject
    {
        public Rope? Rope { get; set; } = null;

        public float ReelSpeed { get; set; } = 0f;

        public RopeAttachment(Guid owner, Vector2 position) 
            : base(owner, position, new(0,0), new(0,0), new StillImageSprite(""))
        {
        }
    }


    public class Rope
    {
        public bool Initialized { get; set; } = false;
        public RopeAttachment Start { get; set; }
        public RopeAttachment? End { get; set; }
        public float SegmentLength { get; set; }
        public List<RopePoint> Points { get; set; } = [];
        public float Tension => GetTension();
        public float Length => _length;


        private float _length;

        private float GetTension()
        {
            if (End == null)
                return 0f;

            return Math.Max(0, Vector2.Distance(Start.WorldPosition, End.WorldPosition) - _length);
        }

        public Rope(RopeAttachment start, float length, RopeAttachment? end = null)
        {
            Start = start;
            End = end;

            var pointCount = Convert.ToInt32(length / 10f);

            _length = length;
            SegmentLength = length/pointCount;
            Points = Enumerable.Range(0, pointCount).Select(x => new RopePoint()).ToList();
        }
    }

    public class RopePoint
    {
        public Vector2 CurrentPosition { get; set; }
        public Vector2 OldPosition { get; set; }
    }
}
