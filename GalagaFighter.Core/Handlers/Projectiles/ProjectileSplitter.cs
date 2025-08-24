using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Projectiles
{
    public interface IProjectileSplitter
    {
        void Split(Projectile projectile, IProjectileController newController);
    }
    public class ProjectileSplitter : IProjectileSplitter
    {
        private readonly IObjectService _objectService;

        public ProjectileSplitter(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void Split(Projectile projectile, IProjectileController newController)
        {
            var split = projectile.Modifiers.CanSplit;
            if (!split)
                return;

            if (projectile.Lifetime < .5f)
                return;

            if (Game.Random.NextDouble() < .99f)
                return;

            // Get the owner of the projectile
            var owner = _objectService.GetOwner(projectile) as Player;
            if (owner == null)
                return;

            // Create a copy of the current projectile's modifiers
            var clonedModifiers = projectile.Modifiers.Clone();
            
            // Keep CanSplit = true for infinite splitting!
            // Both original and clone can continue splitting
            
            // Calculate angles within 45 degrees of the existing trajectory
            var currentAngle = Math.Atan2(projectile.Speed.Y, projectile.Speed.X);
            var maxDeviationRadians = Math.PI / 4; // 45 degrees
            
            // Random angles for both projectiles (original and clone)
            var angle1 = currentAngle + (Game.Random.NextDouble() - 0.5) * maxDeviationRadians;
            var angle2 = currentAngle + (Game.Random.NextDouble() - 0.5) * maxDeviationRadians;
            
            // Calculate new speeds maintaining the same magnitude
            var speed = Math.Sqrt(projectile.Speed.X * projectile.Speed.X + projectile.Speed.Y * projectile.Speed.Y);
            
            // Update current projectile trajectory
            projectile.HurryTo(
                (float)(Math.Cos(angle1) * speed),
                (float)(Math.Sin(angle1) * speed)
            );

            // Create the clone projectile using the provided controller
            if (projectile.Modifiers.OnShootProjectiles.Count > 0)
            {
                var projectileFactory = projectile.Modifiers.OnShootProjectiles[0];
                var cloneProjectile = projectileFactory(newController, owner, projectile.Center, clonedModifiers);

                projectile.Modifiers.CanSplit = false;

                // Set the clone's trajectory
                cloneProjectile.HurryTo(
                    (float)(Math.Cos(angle2) * speed),
                    (float)(Math.Sin(angle2) * speed)
                );

                // Add clone to game objects
                _objectService.AddGameObject(cloneProjectile);
            }
        }
    }
}
