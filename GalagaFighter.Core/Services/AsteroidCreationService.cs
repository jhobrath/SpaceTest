using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IAsteroidCreationService
    {
        void Update();
        void Explode(Asteroid asteroid, Vector2 position);
    }
    public class AsteroidCreationService : IAsteroidCreationService
    {
        private readonly IObjectService _objectService;

        private readonly ConcurrentQueue<(Image, Vector2[])> Queue = new();

        public AsteroidCreationService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        private float _nextDrop = GetRandomDelay();
        private float _sinceLastDrop;

        private static float GetRandomDelay()
        {
            return 1f + 1f * (float)Game.Random.NextDouble();
        }

        public void Update()
        {
            _sinceLastDrop += Raylib.GetFrameTime();

            if (Queue.Count < 10)
            {
                Task.Run(() => Queue.Enqueue(AsteroidSpriteFactory.CreateProceduralAsteroidSpriteWithVertices()));
                return;
            }


            if (_sinceLastDrop < _nextDrop)
                return;

            _nextDrop = GetRandomDelay();
            _sinceLastDrop = 0f;

            var position = GetRandomVector(300, -200, Game.Width - 600, -200);
            var size = GetRandomVector(50, 50, 200, 200);
            var speed = GetRandomVector(-5, 60, 5, 130);

            var asteroid = new Asteroid(position, size, speed);
            _objectService.AddGameObject(asteroid);

            CleanUp();
        }

        private void CleanUp()
        {
            var asteroids = _objectService.GetGameObjects<Asteroid>();
            foreach(var asteroid in asteroids)
                if (asteroid.Rect.Y > Game.Height + 50)
                    asteroid.IsActive = false;
        }

        private Vector2 GetRandomVector(float minX, float minY, float maxX, float maxY)
        {
            return new Vector2(
                minX + (maxX - minX)*(float)Game.Random.NextDouble(),
                minY + (maxY - minY)*(float)Game.Random.NextDouble()
            );
        }

        public void Explode(Asteroid asteroid, Vector2 collisionPosition)
        {
            var numberOfAsteroids = Game.Random.Next(2, 5);
            var averageAsteroidSize = asteroid.Rect.Size/numberOfAsteroids;
            var averageAsteroidSpeed = asteroid.Speed / numberOfAsteroids;
            for(var i =0;i < numberOfAsteroids;i++)
            {
                var position = GetRandomVector(asteroid.Center.X - 50, asteroid.Center.Y - 50, asteroid.Center.X + 50, asteroid.Center.Y + 50);
                var size = GetRandomVector(averageAsteroidSize.X * .8f, averageAsteroidSize.Y * .8f, averageAsteroidSize.X / .8f, averageAsteroidSize.Y / .8f);
                var speed = GetRandomVector(averageAsteroidSpeed.X * .8f, averageAsteroidSpeed.Y * .8f, averageAsteroidSpeed.X / .8f, averageAsteroidSpeed.Y / .8f);

                var asteroidData = Queue.TryDequeue(out (Image, Vector2[]) result);// AsteroidSpriteFactory.CreateProceduralAsteroidSpriteWithVertices();
                if (!asteroidData)
                    break;

                var newAsteroid = new Asteroid(result, position, size, speed);
                _objectService.AddGameObject(newAsteroid);
            }

            asteroid.IsActive = false;
        }
    }
}
