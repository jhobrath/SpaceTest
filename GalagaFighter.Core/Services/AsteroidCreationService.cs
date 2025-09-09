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
        void Initialize();
        void Explode(Asteroid asteroid, Vector2 position, float defaultOpacity = 1f);
    }
    public class AsteroidCreationService : IAsteroidCreationService
    {
        private readonly IObjectService _objectService;

        private int _asteroidCount = 0;

        private readonly ConcurrentQueue<(Image, Vector2[])> Queue = new();

        public AsteroidCreationService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Initialize()
        {
            _asteroidCount = 0;
            for (var i = 0; i < 10; i++)
                GenerateAsteroid(Game.Height * (float)Game.Random.NextDouble() + 200f);
        }

        private float _nextDrop = GetRandomDelay();
        private float _sinceLastDrop;
        private int _startingCount = 0;
        private bool _neverReachedTen = true;

        private static float GetRandomDelay()
        {
            return .5f * (float)Game.Random.NextDouble();
        }

        public void Update()
        {
            _sinceLastDrop += Raylib.GetFrameTime();

            if (Queue.Count < 10)
            {
                Task.Run(() => Queue.Enqueue(AsteroidSpriteFactory.CreateProceduralAsteroidSpriteWithVertices()));
                return;
            }

            if (_sinceLastDrop < _nextDrop && _asteroidCount > 10)
                return;

            _nextDrop = GetRandomDelay();
            _sinceLastDrop = 0f;
            GenerateAsteroid();

            CleanUp();
        }

        private void GenerateAsteroid(float verticalOffset = 0f)
        {
            var size = GetRandomVector(100, 100, 300, 300);


            Vector2 position;
            Vector2 speed;

            if (Game.Random.NextDouble() > .5f)
            {
                position = GetRandomVector(300, verticalOffset - 200, Game.Width - 600, verticalOffset - 200);
                speed = GetRandomVector(-5, 150, 5, 300);
            }
            else
            {
                position = GetRandomVector(300, Game.Height - verticalOffset, Game.Width - 600, Game.Height + 200 - verticalOffset);
                speed = GetRandomVector(-5, -300, 5, -150);
            }


            var asteroidData = Queue.TryDequeue(out (Image, Vector2[]) result);// AsteroidSpriteFactory.CreateProceduralAsteroidSpriteWithVertices();
            if (!asteroidData)
                return;

            var asteroid = new Asteroid(result, position, size, speed);

            _objectService.AddGameObject(asteroid);
        }

        private void CleanUp()
        {
            var asteroids = _objectService.GetGameObjects<Asteroid>();
            _asteroidCount = asteroids.Count(x => x.Opacity == 1f);
            foreach (var asteroid in asteroids)
                if (asteroid.Rect.Y > Game.Height + 50 || asteroid.Rect.Y < -50 || asteroid.Rect.X < -150 || asteroid.Rect.X > Game.Width + 50)
                    asteroid.IsActive = false;
        }

        private Vector2 GetRandomVector(float minX, float minY, float maxX, float maxY)
        {
            return new Vector2(
                minX + (maxX - minX)*(float)Game.Random.NextDouble(),
                minY + (maxY - minY)*(float)Game.Random.NextDouble()
            );
        }

        public void Explode(Asteroid asteroid, Vector2 collisionPosition, float defaultOpacity = 1f)
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
                newAsteroid.Opacity = .99f;
                _objectService.AddGameObject(newAsteroid);
            }

            asteroid.IsActive = false;
        }
    }
}
