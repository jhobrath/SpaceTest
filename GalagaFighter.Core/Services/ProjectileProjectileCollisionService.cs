using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Numerics;

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
            var projectiles = _objectService.GetGameObjects<Projectile>();
            var groupedProjectiles = projectiles.GroupBy(x => x.Owner).ToList();
            if (groupedProjectiles.Count() < 2) return;

            DebugWriter.Write(projectiles.Count.ToString());


            var p1Projectiles = groupedProjectiles.First();
            var p2Projectiles = groupedProjectiles.Last();

            foreach(var p1 in p1Projectiles)
            {
                foreach(var p2 in p2Projectiles)
                {
                    if (!IsNear(p1, p2))
                        continue;

                    var handle = GetHandleMethod(p1, p2);
                    if (handle == null)
                        continue;

                    handle();
                }
            }
        }

        private bool IsNear(Projectile p1, Projectile p2)
        {
            return Vector2.Distance(p1.Center, p2.Center) < 200f;
        }

        private Action? GetHandleMethod(Projectile p1, Projectile p2)
        {
            var p1State = p1.Modifiers.OnNearProjectile?.Any() ?? false;
            var p2State = p2.Modifiers.OnNearProjectile?.Any() ?? false;

            if (p1State && p2State)
            {
                var whoGoesFirst = Game.Random.NextDouble() < .5;
                var first = whoGoesFirst ? p1 : p2;
                var second = whoGoesFirst ? p2 : p1;

                return () =>
                {
                    first.Modifiers.OnNearProjectile!.ForEach(f => f(first, second));
                    second.Modifiers.OnNearProjectile!.ForEach(f => f(second, first));
                };
            }

            if (p1State)
                return () => p1.Modifiers.OnNearProjectile!.ForEach(f => f(p1, p2));

            if (p2State)
                return () => p2.Modifiers.OnNearProjectile!.ForEach(f => f(p2, p1));

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
