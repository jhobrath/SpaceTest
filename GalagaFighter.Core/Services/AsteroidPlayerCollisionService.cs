using GalagaFighter.Core.Handlers.Collisions;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IAsteroidPlayerCollisionService
    {
        void HandleCollisions();
    }
    public class AsteroidPlayerCollisionService : IAsteroidPlayerCollisionService
    {
        private readonly IObjectService _objectService;
        private readonly ICollisionCreationService _collisionCreationService;
        private readonly IAsteroidCreationService _asteroidCreationService;

        public AsteroidPlayerCollisionService(IObjectService objectService, 
            ICollisionCreationService collisionCreationService, 
            IAsteroidCreationService asteroidCreationService)
        {
            _objectService = objectService;
            _collisionCreationService = collisionCreationService;
            _asteroidCreationService = asteroidCreationService;
        }

        public void HandleCollisions()
        {
            var players = _objectService.GetGameObjects<Player>();
            var asteroids = _objectService.GetGameObjects<Asteroid>();

            foreach(var player in players) { 
                foreach(var asteroid in asteroids)
                {
                    if (asteroid.Opacity < 1)
                        continue;

                    if(ContactCollisionDetector.HasCollision(player, asteroid))
                    {
                        var asteroidArea = GetAsteroidArea(asteroid);
                        var damage = asteroidArea / 20f;
                        player.Health -= damage;

                        var collisionCenter = (player.Center + asteroid.Center) / 2;
                        _collisionCreationService.Create(collisionCenter, new Vector2(asteroidArea / 3f, asteroidArea / 3f), asteroid.Speed / 3f);

                        _asteroidCreationService.Explode(asteroid, collisionCenter, defaultOpacity: .99f);
                    }
                }
            }
        }

        private float GetAsteroidArea(Asteroid asteroid)
        {
            if (asteroid.Hitbox?.Vertices == null || asteroid.Hitbox.Vertices.Length < 3)
                return 0f;

            var vertices = asteroid.Hitbox.Vertices;
            
            // Use the shoelace formula to calculate polygon area
            float area = 0f;
            int n = vertices.Length;
            
            for (int i = 0; i < n; i++)
            {
                int j = (i + 1) % n;
                area += vertices[i].X * vertices[j].Y;
                area -= vertices[j].X * vertices[i].Y;
            }
            
            // Get the absolute area and divide by 2 (shoelace formula gives 2x area)
            float normalizedArea = Math.Abs(area) / 2f;
            
            // Scale by the asteroid's actual size since vertices are normalized (0-1)
            float actualArea = normalizedArea * (asteroid.Rect.Width + asteroid.Rect.Height)/2;
            
            return actualArea;
        }
    }
}
