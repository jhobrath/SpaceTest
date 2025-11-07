using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Core2.Controllers
{
    public interface ISpringAttachmentController : IController<SpringAttachment>
    {
    }

    public class SpringAttachmentController : ISpringAttachmentController
    {
        public const int ConstraintIterations = 20;

        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;

        public SpringAttachmentController(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
        }

        public void Update(SpringAttachment attachment, float frameTime)
        {

            var spring = attachment.Spring;
            if (spring == null)
                return;

            Initialize(spring);
            PinEndpoints(spring);

            if (spring.IsCompressed)
            { 
                DistributePointsAlongLength(spring);
                SetExpandRate(spring);
            }
            else
            {
                spring.CurrentLength = spring.RestLength;
                DistributePointsAlongLength(spring);
            }

            ApplyForce(spring, frameTime);
        }

        private void SetExpandRate(Spring spring)
        {
            spring.ExpandRate = (spring.RestLength / spring.CurrentLength) * (2000 - spring.Stiffness);
        }

        public void ApplyForce(Spring spring, float frameTime)
        {
            if (Math.Abs(spring.CurrentLength - spring.RestLength) < 1)
                return;

            var player = _objectService.GetPlayer(spring.Start);
            var vector = Vector2.Normalize(spring.Start.WorldPosition - player.WorldPosition);
            var playerVector = Vector2.Normalize(player.Speed);
            var dotProduct = Vector2.Dot(playerVector, vector);
            bool isMovingAway = dotProduct < 0;

            if (isMovingAway)
            {
                var stiffPct = (1 - spring.CurrentLength / spring.RestLength);
                var speedChange = spring.ExpandRate * -vector;
                player.Hurry(speedChange.X, speedChange.Y);
            }
            else
            {
                var stiffPct = (1 - spring.CurrentLength / spring.RestLength);
                var speedChange = spring.ExpandRate * -vector / 5;
                player.Hurry(speedChange.X, speedChange.Y);
            }
        }

        public void Initialize(Spring spring)
        {
            if (spring.Initialized)
                return;

            spring.Initialized = true;
            DistributePointsAlongLength(spring);
        }

        private void PinEndpoints(Spring spring)
        {
            spring.Points[0].CurrentPosition = 
            spring.Points[0].OldPosition = spring.Start.WorldPosition;

            if(spring.IsCompressed)
            {
                var player = _objectService.GetPlayer(spring.Start);
                var vector = Vector2.Normalize(spring.Start.WorldPosition - player.WorldPosition);
                spring.Points[spring.Points.Count - 1].CurrentPosition =
                spring.Points[spring.Points.Count - 1].OldPosition = spring.Points[0].CurrentPosition + spring.CurrentLength * vector;
            }
        }

        private void DistributePointsAlongLength(Spring spring)
        {
            // Calculate the direction of the spring
            var player = _objectService.GetPlayer(spring.Start);
            var vector = Vector2.Normalize(spring.Start.WorldPosition - player.WorldPosition);
            var segmentLength = spring.CurrentLength / spring.Points.Count;

            for (var i = 0; i < spring.Points.Count; i++)
                spring.Points[i].CurrentPosition = spring.Start.WorldPosition + vector * i * segmentLength;
        }

        public void Draw(SpringAttachment springAttachment, float frameTime)
        {
            var spring = springAttachment.Spring;
            if (spring == null) return;

            float amplitude = spring.CoilAmplitude; // Use property
            int stepsPerSegment = 20;

            for (int i = 0; i <= spring.Points.Count - 2; i++)
            {
                var p1 = spring.Points[i];
                var p2 = spring.Points[i + 1];
                Vector2 start = p1.CurrentPosition;
                Vector2 end = p2.CurrentPosition;
                Vector2 dir = Vector2.Normalize(end - start);
                Vector2 perp = new Vector2(-dir.Y, dir.X);
                float length = Vector2.Distance(start, end);

                // Alternate phase direction for each segment
                float phase = (i % 2 == 0) ? 1f : -1f;

                Vector2 prev = start;
                for (int s = 1; s <= stepsPerSegment; s++)
                {
                    float t = (float)s / stepsPerSegment;
                    Vector2 point = Vector2.Lerp(start, end, t);
                    float wave = MathF.Sin(t * MathF.PI) * amplitude * phase;
                    point += perp * wave;
                    Raylib.DrawLineV(prev, point, Color.Red);
                    prev = point;
                }
            }
        }
    }
}
