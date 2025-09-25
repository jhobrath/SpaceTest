using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;

namespace GalagaFighter.Core.CPU.Gambits
{
    public interface IOpponentBulletWatcher
    {
        DangerScore GetThreat(Player player);
    }

    public class OpponentBulletWatcher : IOpponentBulletWatcher
    {
        private readonly IObjectService _objectService;

        public OpponentBulletWatcher(IObjectService objectService)
        {
            _objectService = objectService;
        }

        public DangerScore GetThreat(Player player)
        {
            var projectileDestinies = GetProjectileDestinies(player);
            return CalculateScore(player, projectileDestinies);
        }

        private static DangerScore CalculateScore(Player player, List<ProjectileDestiny> projectileDestinies)
        {
            var scoresAbove = 0;
            var scoresBelow = 0;
            var scoreAbove = 0f;
            var scoreBelow = 0f;
            foreach (var destiny in projectileDestinies)
            {
                var projScore = CalculateScore(player, destiny);
                if (projScore > 0)
                {
                    scoreAbove += projScore;
                    scoresAbove++;
                }
                else if (projScore < 0)
                {
                    scoreBelow += projScore;
                    scoresBelow++;
                }
            }

            if(scoresAbove != 0)
                scoreAbove /= scoresAbove;

            if(scoresBelow != 0)
                scoreBelow /= scoresBelow;

            return new DangerScore
            {
                Above = scoreAbove,
                Below = Math.Abs(scoreBelow)
            };
        }

        private static float CalculateScore(Player player, ProjectileDestiny destiny)
        {
            if (destiny.TimeLeft > 4f)
                return 0f;
            var dist = Math.Abs(destiny.TargetY - player.Center.Y);
            if (dist > 400)
                return 0f;
            var timePriority = (.35f - destiny.TimeLeft) / .35f;
            var distPriority = (400f - dist) / 400f;
            var projScore = timePriority * distPriority;
            if (destiny.TargetY < player.Center.Y)
                projScore *= -1;
            return projScore;
        }

        private List<ProjectileDestiny> GetProjectileDestinies(Player player)
        {
            var projectiles = _objectService.GetGameObjects<Projectile>();
            var projectileDestinies = new List<ProjectileDestiny>();

            foreach (var projectile in projectiles)
            {
                if (projectile.Owner == player.Id)
                    continue;

                projectileDestinies.Add(GetProjectileDestiny(player.Rect.X, projectile));
            }

            return projectileDestinies;
        }

        private static ProjectileDestiny GetProjectileDestiny(float targetX, Projectile projectile)
        {
            var speed = projectile.Speed;
            var startPoint = projectile.Center;

            // Check for a division by zero to ensure the point is moving horizontally.
            if (Math.Abs(speed.X) < float.Epsilon)
            {
                return new ProjectileDestiny(float.PositiveInfinity, startPoint.Y);
            }

            // Calculate the time it takes to reach the target x-position.
            float time = (targetX - startPoint.X) / speed.X;

            // A negative time means the point is moving away from the target x-position.
            if (time < 0)
            {
                return new ProjectileDestiny(float.PositiveInfinity, startPoint.Y);
            }

            // Calculate the final y-position using the time.
            float yPosition = startPoint.Y + (speed.Y * time);

            return new ProjectileDestiny(time, yPosition);
        }

        private class ProjectileDestiny
        {
            public float TargetY { get; set; }
            public float TimeLeft { get; set; }

            public ProjectileDestiny(float timeLeft, float targetY)
            {
                TargetY = targetY;
                TimeLeft = timeLeft;
            }
        }
    }

    public class DangerScore
    {
        public float Above { get; set; }
        public float Below { get; set; }
    }
}
