using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.CPU.Gambits
{
    public interface IOpponentBulletWatcher
    {
        List<ProjectileDestiny> GetDestinies(Player? player);
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
            var projectileDestinies = GetDestinies(player);
            return CalculateScore(player, projectileDestinies);
        }

        private static DangerScore CalculateScore(Player player, List<ProjectileDestiny> projectileDestinies)
        {
            var scoresAbove = 0;
            var scoresBelow = 0;
            var scoreAbove = 0f;
            var scoreBelow = 0f;

            var dots4 = new List<float>();
            var dots3 = new List<float>();
            var dots2 = new List<float>();
            var dots1 = new List<float>();

            foreach (var destiny in projectileDestinies)
            {
                if (destiny.TimeLeft < .1f)
                { 
                    //Raylib.DrawRectangle((int)player.Center.X - 5, (int)destiny.TargetY, 10, 10, Raylib_cs.Color.Pink);
                    dots1.Add(destiny.TargetY);
                }
                else if (destiny.TimeLeft < .2f)
                { 
                    //Raylib.DrawRectangle((int)player.Center.X - 15, (int)destiny.TargetY, 10, 10, Raylib_cs.Color.Pink);
                    dots2.Add(destiny.TargetY);
                }
                else if (destiny.TimeLeft < .3f)
                { 
                    //Raylib.DrawRectangle((int)player.Center.X - 25, (int)destiny.TargetY, 10, 10, Raylib_cs.Color.Pink);
                    dots3.Add(destiny.TargetY);
                }
                else if (destiny.TimeLeft < .4f)
                { 
                    //Raylib.DrawRectangle((int)player.Center.X - 35, (int)destiny.TargetY, 10, 10, Raylib_cs.Color.Pink);
                    dots4.Add(destiny.TargetY);
                }
                else
                    continue;

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


            void CreateSafeZones(List<float> dots, int xOffset)
            {
                var lastDot = -80;
                dots = dots.OrderBy(x => x).ToList();
                dots.Add(Game.Height + 80);
                foreach (var dot in dots)
                {
                    var lastDotEndUnsafe = lastDot + 80;
                    var thisDotStartUnsafe = (int)dot - 80;

                    if(thisDotStartUnsafe - lastDotEndUnsafe > 0)
                        Raylib.DrawRectangle((int)Game.Width - xOffset - (int)player.Rect.Width/2, lastDotEndUnsafe, 300, thisDotStartUnsafe - lastDotEndUnsafe, Raylib_cs.Color.Green.ApplyAlpha(.5f));

                    lastDot = (int)dot;
                }
            }

            //CreateSafeZones(dots1, 300);
            //CreateSafeZones(dots2, 600);
            //CreateSafeZones(dots3, 900);
            //CreateSafeZones(dots4, 1200);

            var unitHeight = (Game.Height / 200);
            var dangers = new List<float>();
            for (var i = 0; i < 200; i++)
            {
                var yPos = unitHeight * i;
                var danger = 0f;
                foreach (var destiny in projectileDestinies)
                    if(Math.Abs(destiny.TargetY - yPos) < unitHeight/2)
                        danger += Math.Clamp((.4f - destiny.TimeLeft) / .4f, 0, 1);

                dangers.Add(danger);
            }

            List<float> Convolute(List<float> list)
            { 
                var convolutedDangers = new List<float>();
                for(var i = 0;i < list.Count;i++)
                {
                    var newDanger = 0f;
                    if(i - 2 >= 0)
                        newDanger += .3333f * list[i - 2];
                    if (i - 1 >= 0)
                        newDanger += .6667f * list[i - 1];
                    newDanger += list[i];
                    if (i + 1 < list.Count)
                        newDanger += .66667f * list[i + 1];
                    if (i + 2 < list.Count)
                        newDanger += .33333f * list[i + 2];

                    convolutedDangers.Add(newDanger);
                }

                return convolutedDangers;
            }
            dangers = Convolute(dangers);
            dangers = Convolute(dangers);
            dangers = Convolute(dangers);
            dangers = Convolute(dangers);
            dangers = Convolute(dangers);

            for (var i = 0;i < dangers.Count;i++)
            {
                var dotSize = 3;
                var xPos = Game.Width - player.Rect.Width / 2 - (dotSize/2) - dangers[i]*10;
                var yPos = unitHeight * i - (dotSize/2);

                Raylib.DrawRectangle((int)xPos, (int)yPos, dotSize, dotSize, Raylib_cs.Color.Pink);
            }




            //if ((Game.Height + 80) - (lastDot + 80) > 160)
            //    Raylib.DrawRectangle((int)Game.Width - 200, lastDot, 200, ((int)Game.Height) - (lastDot + 80), Raylib_cs.Color.Green);

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

        public List<ProjectileDestiny> GetDestinies(Player player)
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
    }

    public class ProjectileDestiny
    {
        public float TargetY { get; set; }
        public float TimeLeft { get; set; }

        public ProjectileDestiny(float timeLeft, float targetY)
        {
            TargetY = targetY;
            TimeLeft = timeLeft;
        }
    }

    public class DangerScore
    {
        public float Above { get; set; }
        public float Below { get; set; }
    }
}
