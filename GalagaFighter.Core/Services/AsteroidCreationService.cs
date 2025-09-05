using GalagaFighter.Core.Models.Debris;
using Raylib_cs;
using System;
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
    }
    public class AsteroidCreationService : IAsteroidCreationService
    {
        private readonly IObjectService _objectService;

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
    }
}
