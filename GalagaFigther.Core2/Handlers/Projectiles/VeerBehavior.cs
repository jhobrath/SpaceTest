using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Projectiles;
using GalagaFighter.Core2.Services;
using System;
using System.Numerics;

namespace GalagaFighter.Core2.Handlers.Projectiles
{
    public class VeerBehavior : ProjectileBehaviorBase
    {
        private readonly IGameDataRegistry _gameDataRegistry;
        public VeerBehavior(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }
        public override void Update(Projectile projectile, float frameTime)
        {
            var veerState = _gameDataRegistry.Get<VeerState>(projectile);
            if (veerState.Veer == 0f)
                return;
            var veerData = _gameDataRegistry.Get<ProjectileVeerState>(projectile);
            if (!veerData.Initialized)
            {
                veerData.OriginalSpeed = projectile.Speed;
                var random = new Random(Guid.NewGuid().GetHashCode());
                float curveStrength = (float)(random.NextDouble() * 2.0 - 1.0) * veerState.Veer;
                veerData.CurrentVeer = new Vector2(curveStrength, 0);
                veerData.Initialized = true;
            }
            float speedMag = projectile.Speed.Length();
            if (speedMag == 0f) return;
            float curveStrengthUsed = veerData.CurrentVeer.X;
            float angle = (curveStrengthUsed / speedMag) * frameTime;
            float cos = MathF.Cos(angle);
            float sin = MathF.Sin(angle);
            var v = projectile.Speed;
            var rotated = new Vector2(
                v.X * cos - v.Y * sin,
                v.X * sin + v.Y * cos
            );
            projectile.HurryTo(rotated.X, rotated.Y);
        }
    }
}
