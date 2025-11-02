using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public interface IProjectileShooter
    {
        void Shoot(GameObject gun, Projectile projectile, GunBarrel barrel);
    }

    public class ProjectileShooter : IProjectileShooter
    {
        private readonly IObjectService _objectService;
        private readonly IGameObjectPositionService _gameObjectPositionService;

        public ProjectileShooter(IObjectService objectService, IGameObjectPositionService gameObjectPositionService)
        {
            _objectService = objectService;
            _gameObjectPositionService = gameObjectPositionService;
        }

        public void Shoot(GameObject objectShooting, Projectile projectile, GunBarrel barrel)
        {
            var barrelStart = GetRotatedOffset(objectShooting, barrel.Start);
            var barrelEnd = GetRotatedOffset(objectShooting, barrel.End);

            if (projectile.IsTransformChild)
            { 
                projectile.MoveTo(barrel.End.X, barrel.End.Y);
                _gameObjectPositionService.RegisterParent(objectShooting, projectile);
            }
            else
            { 
                MoveInPlace(objectShooting, projectile, barrelStart, barrelEnd);
            }

            Poof(objectShooting, barrelEnd);
            AddEmitters(projectile);

            _objectService.Add(projectile);
        }

        private void AddEmitters(Projectile projectile)
        {
            foreach(var config in projectile.EmitterConfigurations)
            {
                var emitter = new ParticleEmitter(projectile.Id, new(0, 0), 5f) { Config = config };
                _objectService.Add(emitter);
                _gameObjectPositionService.RegisterParent(projectile, emitter);
            }
        }

        private void Poof(GameObject objectShooting, Vector2 position)
        {
            var config = ParticleEffectTemplates.Get("SmokeTrail1");
            config.Loop = false;
            config.Duration = .15f;
            config.Lifetime = .25f;
            config.Speed = new(0f, -400f);
            config.SpeedVariation = new(200f, 100f);
            config.StartSize = 3f;
            config.EmissionRate = 50f;
            config.EndSize = 5f;
            config.StartColor = Color.Orange;
            config.EndColor = Color.Black.ApplyAlpha(0);
            config.Textures = ["dot_1", "dot_2", "dot_3"];

            var emitter = new ParticleEmitter(Game.Id, position - config.StartSize*Vector2.One/2, 10) { Config = config, Rotation = objectShooting.WorldRotation };

            _objectService.Add(emitter);
        }

        private static void MoveInPlace(GameObject objectShooting, Projectile projectile, Vector2 barrelStart, Vector2 barrelEnd)
        {
            var barrelVector = barrelEnd - barrelStart;
            var barrelRotationRadians = MathF.Atan2(-barrelVector.Y, barrelVector.X);

            var speedLength = projectile.Speed.Length();
            var speedXPct = MathF.Cos(barrelRotationRadians);
            var speedYPct = -MathF.Sin(barrelRotationRadians);
            projectile.HurryTo(speedLength * speedXPct, speedLength * speedYPct);

            var accelLength = projectile.Acceleration.Length();
            var accelXPct = MathF.Cos(barrelRotationRadians);
            var accelYPct = -MathF.Sin(barrelRotationRadians);
            projectile.AccelTo(accelLength * accelXPct, accelLength * accelYPct);


            if (!projectile.IsTransformChild)
            { 
                var theta = MathF.Atan2(projectile.Speed.Y, projectile.Speed.X);
                var rotation = theta * 180.0f / MathF.PI;
                projectile.Rotation = rotation;
            }

            var finalPosition = new Vector2(barrelEnd.X, barrelEnd.Y);
            if (projectile.IsTransformChild)
            {
                //projectile.Rotation -= 90;
                finalPosition -= objectShooting.WorldPosition;
            }

            projectile.MoveTo(finalPosition.X, finalPosition.Y);
        }

        private Vector2 GetRotatedOffset(GameObject objectShooting, Vector2 offset)
        {
            // Now GunBarrel offsets are defined for Raylib 0° (upwards), so just rotate by WorldRotation
            var offsetRotationRadians = objectShooting.WorldRotation * MathF.PI / 180f;
            return objectShooting.Center + new Vector2(
                (offset.X * MathF.Cos(offsetRotationRadians) - offset.Y * MathF.Sin(offsetRotationRadians)),
                (offset.X * MathF.Sin(offsetRotationRadians) + offset.Y * MathF.Cos(offsetRotationRadians))
            );
        }
    }
}
