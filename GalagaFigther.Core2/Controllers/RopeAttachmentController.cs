using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core2.Controllers
{
    public interface IRopeAttachmentController : IController<RopeAttachment>
    {
    }

    public class RopeAttachmentController : IRopeAttachmentController
    {
        public const int ConstraintIterations = 20;

        private readonly IObjectService _objectService;

        public RopeAttachmentController(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Update(RopeAttachment ropeAttachment, float frameTime)
        {
            var rope = ropeAttachment.Rope;
            if (rope == null)
                return;

            Initialize(rope);
            PinEndpoints(rope);
            ApplyIntegration(rope, frameTime);
            SolveContraints(rope, 10);
            Tense(rope);

            rope.Initialized = true;
        }

        private void Tense(Rope rope)
        {
            var tension = rope.Tension;
            if (tension == 0)
                return;

            var player = _objectService.GetPlayer(rope.End!);


            var diff = rope.End!.WorldPosition - rope.Start.WorldPosition;
            var vector = Vector2.Normalize(diff);
            var accelToAdd = vector * tension*3f;
            player.Hurry(-accelToAdd.X, -accelToAdd.Y);


            var angleRad = MathF.Atan2(-diff.Y, diff.X);
            float angleDeg = angleRad * (180f / MathF.PI);
            float raylibAngle = 180f + -angleDeg + 90f;

            while (player.Rotation > 180) player.Rotation -= 360f;
            while (raylibAngle > 180) raylibAngle -= 360f;
            while (player.Rotation < -180) player.Rotation += 360f;
            while (raylibAngle < -180) raylibAngle += 360f;
            player.Rotation =raylibAngle;
        }

        private void Initialize(Rope rope)
        {
            if (rope.Initialized)
                return;

            if (rope.End == null)
            {
                rope.Points.ForEach(x => x.CurrentPosition = x.OldPosition = rope.Points[0].CurrentPosition);
                return;
            }

            MakeStraight(rope);
        }

        private void Reel(float frameTime)
        {

        }

        private void MakeStraight(Rope rope)
        {
            rope.Points[0].CurrentPosition = rope.Points[0].OldPosition = rope.Start.WorldPosition;
            var averageSegmentDistance = (rope.End!.WorldPosition - rope.Start.WorldPosition) / (rope.Points.Count - 1);
            for (var i = 1; i < rope.Points.Count; i++)
                rope.Points[i].OldPosition = rope.Points[i].CurrentPosition = rope.Points[i - 1].CurrentPosition + averageSegmentDistance;
        }

        private void PinEndpoints(Rope rope)
        {
            Vector2 startAttachmentPos = rope.Start.WorldPosition;
            rope.Points[0].CurrentPosition = rope.Points[0].OldPosition = startAttachmentPos;

            if (rope.End != null)
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
            if(rope.Tension > 0)
            {
                var originalPoints = rope.Points.Select(x => x.CurrentPosition).ToArray();
                MakeStraight(rope);

                for (var i = 0; i < rope.Points.Count; i++)
                {
                    var distance = (rope.Points[i].CurrentPosition - originalPoints[i]);
                    var pct = (float)i / (float)rope.Points.Count;
                    var amountToAffect = distance / 3f;// (1 + (4f * pct));

                    rope.Points[i].CurrentPosition = rope.Points[i].OldPosition = originalPoints[i] + amountToAffect;
                }



                return;
            }

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
                    //if (difference < 0) // Only correct if compressed
                    {
                        var correctionVector = Vector2.Normalize(delta) * difference;

                        // Calculate the total inverse mass (w) for this segment
                        // NOTE: Since your endpoints are pinned in step A, their effective mass is infinite, 
                        //       so their inverse mass is 0.
                        var w1 = 1000f;//p1.Mass; // 1 / m1
                        var w2 = 1000f;//p2.Mass; // 1 / m2
                        var totalInvMass = w1 + w2;

                        //TODO: Add Mass to points
                        if (totalInvMass == 0.0f)
                            continue;

                        // Apply the correction weighted by inverse mass
                        // The lighter point moves more than the heavier one.
                        var correctionFactor = 1.0f / totalInvMass;

                        if (w1 > 0) // If p1 is not fixed (mass > 0)
                            // Move p1 towards p2
                            p1.CurrentPosition += correctionVector * (w1 * correctionFactor * 0.65f);


                        if (w2 > 0) // If p2 is not fixed (mass > 0)
                            // Move p2 away from p1
                            p2.CurrentPosition -= correctionVector * (w2 * correctionFactor * 0.65f);
                    }
                }
                //rope.Points[0].CurrentPosition = rope.Points[0].OldPosition = rope.Start.WorldPosition;
                //if(rope.End != null)
                //    rope.Points[rope.Points.Count - 1].CurrentPosition = rope.Points[rope.Points.Count - 1].OldPosition = rope.End!.WorldPosition;
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
