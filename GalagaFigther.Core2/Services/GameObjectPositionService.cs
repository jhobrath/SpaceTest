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

                    ApplyParentTransform(parent, child);
                }

                childInactiveKeys.ForEach(x => _transformHierarchy[parent].Remove(x));
            }

            inactiveKeys.ForEach(x => _transformHierarchy.Remove(x));
        }

        private void ApplyParentTransform(GameObject parent, GameObject child)
        {
            // Apply rotation: parent's world rotation + child's current rotation (which includes AngularVelocity effects)
            child.WorldRotation = parent.WorldRotation + child.Rotation;

            // The child's Rect.Position represents its local offset from the parent's top-left
            Vector2 localOffset = child.Rect.Position;
            
            // If there's no offset, child follows parent position
            if (localOffset == Vector2.Zero)
            {
                child.WorldPosition = parent.WorldPosition;
                return;
            }

            // Convert local offset (from parent's top-left) to offset from parent's center
            Vector2 parentHalfSize = new Vector2(parent.Width, parent.Height) / 2f;
            Vector2 offsetFromParentCenter = localOffset - parentHalfSize;

            // Rotate the offset around parent's center using parent's world rotation (not child's)
            float parentRotationRadians = parent.WorldRotation * MathF.PI / 180f;
            float cos = MathF.Cos(parentRotationRadians);
            float sin = MathF.Sin(parentRotationRadians);
            
            Vector2 rotatedOffsetFromCenter = new Vector2(
                offsetFromParentCenter.X * cos - offsetFromParentCenter.Y * sin,
                offsetFromParentCenter.X * sin + offsetFromParentCenter.Y * cos
            );

            // Calculate final world position: parent center + rotated offset
            Vector2 childCenter = parent.Center + rotatedOffsetFromCenter;
            
            // Convert back to child's top-left world position
            Vector2 childHalfSize = new Vector2(child.Width, child.Height) / 2f;
            child.WorldPosition = childCenter - childHalfSize;
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
            
            // Initialize child transforms immediately
            foreach(var child in children)
            {
                ApplyParentTransform(gameObject, child);
            }
        }
    }
}
