using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IGameObjectPositionService
    {
        public void RegisterParent(GameObject gameObject, params GameObject[] child);
        public void Update(float frameTime);
    }
    public class GameObjectPositionService : IGameObjectPositionService
    {
        private readonly IObjectService _objectService;

        private readonly Dictionary<GameObject, List<GameObject>> _transformHierarchy = [];

        public GameObjectPositionService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Update(float frameTime)
        {
            var gameObjects = _objectService.GetAll();
            foreach(var gameObject in gameObjects)
            {
                Hurry(gameObject, frameTime);
                Move(gameObject, frameTime);
                Rotate(gameObject, frameTime);
                
                gameObject.WorldPosition = gameObject.Rect.Position;
                gameObject.WorldRotation = gameObject.Rotation;
            }

            var inactiveKeys = new List<GameObject>();
            foreach (var parent in _transformHierarchy.Keys)
            {
                if (parent.IsActive == false)
                { 
                    inactiveKeys.Add(parent);
                    continue;
                }

                var childInactiveKeys = new List<GameObject>();
                foreach (var child in _transformHierarchy[parent])
                {
                    if (child.IsActive == false)
                    {
                        childInactiveKeys.Add(child);
                        continue;
                    }

                    child.WorldPosition += parent.WorldPosition;
                    child.WorldRotation += parent.WorldRotation;
                }

                childInactiveKeys.ForEach(x => _transformHierarchy[parent].Remove(x));
            }

            inactiveKeys.ForEach(x => _transformHierarchy.Remove(x));
        }

        private void Rotate(GameObject gameObject, float frameTime)
        {
            if(gameObject.AngularVelocity != 0f)
            {
                gameObject.Rotation += gameObject.AngularVelocity * frameTime;
            }
        }

        private static void Hurry(GameObject gameObject, float frameTime)
        {
            //if (gameObject.Acceleration == Vector2.Zero)
            //    return;

            var speedDeltaX = gameObject.Acceleration.X * frameTime;
            var speedDeltaY = gameObject.Acceleration.Y * frameTime;

            // Different air resistance when accelerating vs coasting
            var isAcceleratingX = Math.Abs(gameObject.Acceleration.X) > 0.1f;
            var isAcceleratingY = Math.Abs(gameObject.Acceleration.Y) > 0.1f;

            // Lower resistance when accelerating, higher when coasting
            var airResistanceCoefficientX = isAcceleratingX ? 1f : gameObject.Drag.X;
            var airResistanceCoefficientY = isAcceleratingY ? 1f : gameObject.Drag.Y;

            var airResistanceX = -airResistanceCoefficientX * gameObject.Speed.X * frameTime;
            var airResistanceY = -airResistanceCoefficientY * gameObject.Speed.Y * frameTime;

            var newSpeedX = gameObject.Speed.X + speedDeltaX + airResistanceX;
            var newSpeedY = gameObject.Speed.Y + speedDeltaY + airResistanceY;

            gameObject.HurryTo(newSpeedX, newSpeedY);
        }

        private static void Move(GameObject gameObject, float frameTime)
        {
            if (gameObject.Speed == Vector2.Zero)
                return;

            gameObject.Move(gameObject.Speed.X * frameTime, gameObject.Speed.Y * frameTime);
        }

        public void RegisterParent(GameObject gameObject, params GameObject[] children)
        {
            if(!_transformHierarchy.ContainsKey(gameObject))
                _transformHierarchy.Add(gameObject, []);

            _transformHierarchy[gameObject].AddRange(children);
        }
    }
}
