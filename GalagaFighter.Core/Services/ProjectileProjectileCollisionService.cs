using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Handlers.Collisions;
using System.Linq;
using System;
using System.Numerics;
using System.Collections.Generic;

namespace GalagaFighter.Core.Services
{
    public interface IProjectileProjectileCollisionService
    {
        void HandleCollisions();
    }

    public class ProjectileProjectileCollisionService : IProjectileProjectileCollisionService
    {
        private readonly IObjectService _objectService;

        public ProjectileProjectileCollisionService(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public void HandleCollisions()
        {
            var projectiles = _objectService.GetGameObjects<Projectile>()
                .GroupBy(x => x.Owner).ToList();
            if (projectiles.Count() < 2) return;
            var projectilePairs = projectiles
                .First()
                .Zip(projectiles.Last(), (a, b) => new { P1 = a, P2 = b })
                .Where(p => IsNear(p.P1,p.P2))
                .ToList();

            var players = new Lazy<List<Player>>(_objectService.GetGameObjects<Player>);

            foreach (var pair in projectilePairs)
            {
                var handle = GetHandleMethod(pair.P1, pair.P2, players);
                if (handle == null)
                {
                    if(pair.P1.Owner != players.Value.Single(x => x.IsPlayer1).Id)
                    {
                        var s = "";
                    }
                    continue;
                }

                handle();

                //pair.P1.Modifiers.OnNearProjectile = null;
                //pair.P2.Modifiers.OnNearProjectile = null;
            }
        }

        private bool IsNear(Projectile p1, Projectile p2)
        {
            //var pyth = (Vector2 v) => Math.Sqrt(Math.Pow(v.X, 2) + Math.Pow(v.Y, 2));
            //var actualDistance = pyth(p1.Center - p2.Center);
            //if (actualDistance < 100f)
            //    return true;
            //
            //return false;
            var distX = Math.Abs(p1.Center.X - p2.Center.X);
            var distY = Math.Abs(p1.Center.Y - p2.Center.Y);

            return distX < 40 && distY < 200 && (distX + distY) < 200;
        }

        private Action? GetHandleMethod(Projectile p1, Projectile p2, Lazy<List<Player>> players)
        {
            var p1State = p1.Modifiers.OnNearProjectile != null;
            var p2State = p2.Modifiers.OnNearProjectile != null;

            Player GetPlayer(Projectile proj) => 
                players.Value.First(p => p.Id == proj.Owner);   

            if (p1State && p2State)
                return Game.Random.NextDouble() <  .5
                   ? () => p1.Modifiers.OnNearProjectile!(p1,p2, GetPlayer(p1), GetPlayer(p2))
                   : () => p2.Modifiers.OnNearProjectile!(p2,p1, GetPlayer(p2), GetPlayer(p1));

            if (p1State)
                return () => p1.Modifiers.OnNearProjectile!(p1, p2, GetPlayer(p1), GetPlayer(p2));

            if (p2State)
                return () => p2.Modifiers.OnNearProjectile!(p2, p1, GetPlayer(p2), GetPlayer(p1));

            return null;
        }

        private enum ProjectileHandleNearState
        {
            Both,
            Projectile1,
            Projectile2,
            None
        }
    }
}
