using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Models.Projectiles;
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
                Drag(gameObject, frameTime);
                Move(gameObject, frameTime);
                Rotate(gameObject, frameTime);

                if(gameObject is Player && gameObject.Id == Game.Player1Id)
                {
                    var s = "";
                }

                gameObject.WorldPosition = gameObject.Rect.Position;// + gameObject.Rect.Size / 2f;
                gameObject.WorldRotation = gameObject.Rotation;
            }

            var inactiveKeys = new List<GameObject>();
            foreach (var parent in _transformHierarchy.Keys)
            {

                if (parent is Player && parent.Id == Game.Player1Id)
                {
                    var s = "";
                }

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

        private void Drag(GameObject gameObject, float frameTime)
        {
            var decelX = gameObject.Drag.X * frameTime;
            var decelY = gameObject.Drag.Y * frameTime;
            var vx = gameObject.Speed.X;
            var vy = gameObject.Speed.Y;


            if (vx > 0)
                vx = MathF.Max(0, vx - decelX);
            else if (vx < 0)
                vx = MathF.Min(0, vx + decelX);

            if (vy > 0)
                vy = MathF.Max(0, vy - decelY);
            else if (vy < 0)
                vy = MathF.Min(0, vy + decelY);

            gameObject.HurryTo(vx, vy);
        }

        private void ApplyParentTransform(GameObject parent, GameObject child)
        {
            // Apply rotation: parent's world rotation + child's current rotation
            child.WorldRotation = parent.WorldRotation + child.Rotation;



            // The child's Rect.Position is a local offset from the parent's center, representing the child's center
            Vector2 localOffset = child.Rect.Position;

            // Raylib rotates clockwise and 0 degrees is up, so convert to math coordinates (0 = right, CCW) and negate for CW
            float raylibRotationDegrees = parent.WorldRotation;
            float parentRotationRadians = raylibRotationDegrees * MathF.PI / 180f;
            float cos = MathF.Cos(parentRotationRadians);
            float sin = MathF.Sin(parentRotationRadians);

            Vector2 rotatedOffset = new Vector2(
                localOffset.X * cos - localOffset.Y * sin,
                localOffset.X * sin + localOffset.Y * cos
            );

            // World position = parent center + rotated offset (child's center is anchored to parent's center)
            child.WorldPosition = parent.Center + rotatedOffset;
        }

        private void Rotate(GameObject gameObject, float frameTime)
        {
            if(gameObject is JackInTheBoxProjectile)
            {
                var s = "";
            }

            if(gameObject.AngularVelocity != 0f)
            {
                gameObject.Rotation += gameObject.AngularVelocity * frameTime;
            }
        }

        private static void Hurry(GameObject gameObject, float frameTime)
        {
            if (gameObject.Acceleration == Vector2.Zero)
                return;

            var speedDeltaX = gameObject.Acceleration.X * frameTime;
            var speedDeltaY = gameObject.Acceleration.Y * frameTime;
            gameObject.Hurry(speedDeltaX, speedDeltaY);
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
