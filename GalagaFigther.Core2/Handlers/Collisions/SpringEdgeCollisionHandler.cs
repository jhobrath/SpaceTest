using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Collisions
{
    public interface ISpringEdgeCollisionHandler
    {
        void Handle(Spring spring);
    }

    public class SpringEdgeCollisionHandler : ISpringEdgeCollisionHandler
    {
        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;

        public SpringEdgeCollisionHandler(IObjectService objectService, IGameDataRegistry gameDataRegistry)
        {
            _objectService = objectService;
            _gameDataRegistry = gameDataRegistry;
        }

        public void Handle(Spring spring)
        {
            var player = _objectService.GetPlayer(spring.Start);
            var screenData = _gameDataRegistry.Get<PlayerBoundsData>(player);
            var lastPoint = spring.Points[spring.Points.Count - 1].CurrentPosition;
            if (lastPoint.X > 15 && lastPoint.Y > 15 && lastPoint.Y < screenData.Max.Y -84)
            {
                spring.IsCompressed = false;
                return;
            }

            var vector = Vector2.Normalize(spring.Start.WorldPosition - player.WorldPosition);
            var distanceToEdge = GetDistanceToEdge(spring.Start.WorldPosition, vector, screenData.Min, screenData.Max);
            spring.CurrentLength = distanceToEdge;
            spring.IsCompressed = true;
        }

        private float GetDistanceToEdge(Vector2 worldPosition, Vector2 vector, Vector2 min, Vector2 max)
        {
            var length = new Vector2(0,0);
            var finalPosition = worldPosition + length;
            while((finalPosition.X > 15) && (finalPosition.Y > 15)  && (finalPosition.Y < max.Y - 84))
            {
                length += vector;
                finalPosition = worldPosition + length;
            }

            return length.Length();
        }
    }
}
