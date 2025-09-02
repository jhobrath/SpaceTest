using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System.Linq;
using System.Numerics;

namespace GalagaFighter.Core.Handlers.Projectiles
{
    public interface IProjectileMover
    {
        IProjectileMover Create();
        void Move(Projectile projectile);
    }
    public class ProjectileMover : IProjectileMover
    {
        private IProjectileMoverWindUpper _projectileMoverWindUpper;
        private IProjectileMoverPlanker _projectileMoverPlanker;
        private IProjectileRotator _projectileRotator;
        private IObjectService _objectService;

        public ProjectileMover(IProjectileMoverWindUpper projectileMoverWindUpper, IProjectileMoverPlanker projectileMoverPlanker,
            IProjectileRotator projectileRotator, IObjectService objectService)
        {
            _projectileMoverWindUpper = projectileMoverWindUpper;
            _projectileMoverPlanker = projectileMoverPlanker;
            _projectileRotator = projectileRotator;
            _objectService = objectService;
        }

        public IProjectileMover Create()
        {
            return new ProjectileMover(_projectileMoverWindUpper.Create(), _projectileMoverPlanker.Create(), _projectileRotator, _objectService);
        }

        public void Move(Projectile projectile)
        {
            var frameTime = Raylib.GetFrameTime();

            if (projectile.Modifiers.WindUpDuration > 0)
                _projectileMoverWindUpper.WindUp(projectile);
            else if (projectile.Modifiers.PlankDuration > 0)
                _projectileMoverPlanker.Plank(projectile);

            if (projectile.Modifiers.Homing != 0f)
                AdjustFromHoming(projectile);

            projectile.Modifiers.VerticalPositionOffset += projectile.Modifiers.VerticalPositionIncrement * frameTime;

            projectile.Move(projectile.Speed.X * projectile.Modifiers.SpeedMultiplier * frameTime, projectile.Speed.Y * frameTime + projectile.Modifiers.VerticalPositionOffset*frameTime);
        }

        private void AdjustFromHoming(Projectile projectile)
        {
            var target = _objectService.GetGameObjects<Player>().Where(p => p.Id != projectile.Owner).Single();
            var direction = Vector2.Normalize(target.Center - projectile.Center);
            var homingStrength = projectile.Modifiers.Homing * projectile.BaseSpeed.X * projectile.Modifiers.SpeedMultiplier * Raylib.GetFrameTime();
            var newSpeed = new Vector2(
                projectile.Speed.X + direction.X * homingStrength,
                projectile.Speed.Y + direction.Y * homingStrength);

            // Limit the speed to the original speed magnitude
            var originalSpeedMagnitude = projectile.Speed.Length();
            if (newSpeed.Length() > originalSpeedMagnitude)
            {
                newSpeed = Vector2.Normalize(newSpeed) * originalSpeedMagnitude;
            }
            projectile.HurryTo(y: newSpeed.Y);
        }
    }
}
