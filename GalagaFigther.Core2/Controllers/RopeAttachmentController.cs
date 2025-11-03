using GalagaFighter.Core2.Handlers.Rope;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core2.Controllers
{
    public interface IRopeAttachmentController : IController<RopeAttachment>
    {
    }

    public class RopeAttachmentController : IRopeAttachmentController
    {
        public const int ConstraintIterations = 10;

        public void Update(RopeAttachment ropeAttachment, float frameTime)
        {
            var rope = ropeAttachment.Rope;
            if (rope == null)
                return;

            if(!rope.Initialized)
                rope.Points.ForEach(x => x.CurrentPosition = x.OldPosition = rope.Points[0].CurrentPosition);

            PinEndpoints(rope);
            ApplyIntegration(rope, frameTime);
            SolveContraints(rope, 10);

            rope.Initialized = true;
        }

        private void PinEndpoints(Rope rope)
        {
            Vector2 startAttachmentPos = rope.Start.WorldPosition;
            rope.Points[0].CurrentPosition = 
            rope.Points[0].OldPosition = startAttachmentPos;

            if(rope.End != null)
            {
                Vector2 endAttachmentPos = rope.End.WorldPosition;
                rope.Points[rope.Points.Count - 1].CurrentPosition = endAttachmentPos;
                rope.Points[rope.Points.Count - 1].OldPosition = endAttachmentPos;
            }
        }

        private void ApplyIntegration(Rope rope, float frameTime)
        {
            var end = rope.End == null ? rope.Points.Count - 1 : rope.Points.Count - 2;

            for(var i = 1;i <= end;i++)
            {
                var p = rope.Points[i];
                Vector2 velocity = p.CurrentPosition - p.OldPosition;
                p.OldPosition = p.CurrentPosition;
                p.CurrentPosition += velocity * .98f;
            }
        }

        private void SolveContraints(Rope rope, int interations)
        {
            for(var iteration = 0;iteration < ConstraintIterations;iteration++)
            {
                for(var i = 0; i <= rope.Points.Count - 2;i++)
                {
                    var p1 = rope.Points[i];
                    var p2 = rope.Points[i + 1];
                    float desiredDist = rope.SegmentLength;

                    // Calculate the correction vector
                    var delta = p2.CurrentPosition - p1.CurrentPosition;
                    var currentDist = delta.Length();
                    var difference = currentDist - desiredDist;

                    // Normalized vector along the rope
                    var correctionVector = Vector2.Normalize(delta) * difference;

                    // Calculate the total inverse mass (w) for this segment
                    // NOTE: Since your endpoints are pinned in step A, their effective mass is infinite, 
                    //       so their inverse mass is 0.
                    var w1 = 10f;//p1.Mass; // 1 / m1
                    var w2 = 10f;//p2.Mass; // 1 / m2
                    var totalInvMass = w1 + w2;

                    //TODO: Add Mass to points
                    if (totalInvMass == 0.0f)
                        continue;

                    // Apply the correction weighted by inverse mass
                    // The lighter point moves more than the heavier one.
                    var correctionFactor = 1.0f / totalInvMass;

                    if (w1 > 0) // If p1 is not fixed (mass > 0)
                        // Move p1 towards p2
                        p1.CurrentPosition += correctionVector * (w1 * correctionFactor * 0.5f);


                    if (w2 > 0) // If p2 is not fixed (mass > 0)
                        // Move p2 away from p1
                        p2.CurrentPosition -= correctionVector * (w2 * correctionFactor * 0.5f);
                }
            }
        }

        public void Draw(RopeAttachment ropeAttachment, float frameTime)
        {
            var rope = ropeAttachment.Rope;
            if (rope == null) return;

            for(var i = 0;i <= rope.Points.Count - 2;i++)
            {
                var p1 = rope.Points[i];
                var p2 = rope.Points[i + 1];

                Raylib.DrawLineV(p1.CurrentPosition, p2.CurrentPosition, Color.Red);
            }
        }
    }
}
