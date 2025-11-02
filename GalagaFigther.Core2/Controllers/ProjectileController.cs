using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Game;
using GalagaFighter.Core2.Models.Particles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Models.Projectiles;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Controllers
{
    public interface IProjectileController : IController<Projectile>
    {

    }

    public class ProjectileController : IProjectileController 
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        private readonly IObjectService _objectService;

        public ProjectileController(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Draw(Projectile projectile, float frameTime)
        {
            projectile.Sprite.Update(frameTime);
            //This is necessary because projectile sprites are drawn for a ship with 90 degree rotation.
            //TODO: Remake projectile images so they are vertical by default
            if(projectile.IsTransformChild)
            {
                projectile.Sprite.Draw(projectile, projectile.WorldRotation - 90f);
            }
            else
            {
                projectile.Sprite.Draw(projectile);
            }

        }

        public void Update(Projectile projectile, float frameTime)
        {
            Rotate(projectile);
            Home(projectile, frameTime);
            Veer(projectile, frameTime);
            Deactivate(projectile, frameTime);
        }

        private void Home(Projectile projectile, float frameTime)
        {
            if (projectile.Homing == 0)
                return;

            var player = _objectService.Get<Player>(projectile.Owner);
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);
            var opponent = _objectService.GetOpponent(player);

            var finalHomingFactor = (projectile.Homing + modifiers.HomingFactor) * frameTime*3.0f;

            var currentSpeed = projectile.Speed.Length();
            var normalizedSpeed = Vector2.Normalize(projectile.Speed);
            var homingSpeed = Vector2.Normalize(-(projectile.WorldPosition - opponent.WorldPosition));

            var finalNormalizedSpeed = Vector2.Normalize(homingSpeed * finalHomingFactor + normalizedSpeed * (1-finalHomingFactor));
            var finalSpeed = finalNormalizedSpeed * currentSpeed;
            projectile.HurryTo(finalSpeed.X, finalSpeed.Y);
        }

        private void Rotate(Projectile projectile)
        {
            var theta = MathF.Atan2(projectile.Speed.Y, projectile.Speed.X);
            var rotation = theta * 180.0f / MathF.PI;
            projectile.Rotation = rotation;
        }

        private void Deactivate(Projectile projectile, float frameTime)
        {
            var screenData = _gameDataRegistry.Get<GameState>();
            if (projectile.WorldPosition.X < -50 || projectile.WorldPosition.X > screenData.ScreenSize.X + 50)
                projectile.IsActive = false;
            if (projectile.WorldPosition.Y < -50 || projectile.WorldPosition.Y > screenData.ScreenSize.Y + 50)
                projectile.IsActive = false;

            if(projectile.Lifetime != -1f)
            {
                projectile.Lifetime -= frameTime;
                if(projectile.Lifetime < 0f)
                {
                    projectile.IsActive = false;
                }    
            }

            if(!projectile.IsActive && projectile is ShotGunShellProjectile)
            {
                var childEmitters = _objectService.GetChildren<ParticleEmitter>(projectile).ToList();
                childEmitters.ForEach(x => x.IsActive = false);
            }
        }

        private void Veer(Projectile projectile, float frameTime)
        {
            if (projectile.Veer == 0f)
                return;

            var veerState = _gameDataRegistry.Get<GalagaFighter.Core2.Models.Projectiles.ProjectileVeerState>(projectile);
            if (!veerState.Initialized)
            {
                veerState.OriginalSpeed = projectile.Speed;
                // Pick a random float in [-Veer, +Veer] for unique curve per projectile
                var random = new Random(Guid.NewGuid().GetHashCode());
                float curveStrength = (float)(random.NextDouble() * 2.0 - 1.0) * projectile.Veer;
                veerState.CurrentVeer = new System.Numerics.Vector2(curveStrength, 0);
                veerState.Initialized = true;
            }

            float speedMag = projectile.Speed.Length();
            if (speedMag == 0f) return;
            float curveStrengthUsed = veerState.CurrentVeer.X;
            float angle = (curveStrengthUsed / speedMag) * frameTime; // radians
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            var v = projectile.Speed;
            var rotated = new System.Numerics.Vector2(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos
            );
            projectile.HurryTo(rotated.X, rotated.Y);
        }
    }
}
