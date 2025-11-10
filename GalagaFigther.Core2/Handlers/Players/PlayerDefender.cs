using GalagaFighter.Core2.Effects.Defensives;
using GalagaFighter.Core2.Effects.Statuses;
using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models.Players;
using GalagaFighter.Core2.Services;
using GalagaFighter.Core2.Services.Static;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Handlers.Players
{
    public interface IPlayerDefender
    {
        void Defend(Player player, float frameTime);
    }
    public class PlayerDefender : IPlayerDefender
    {
        private readonly IObjectService _objectService;
        private readonly IGameDataRegistry _gameDataRegistry;

        public PlayerDefender(IGameDataRegistry gameDataRegistry, IObjectService objectService)
        {
            _gameDataRegistry = gameDataRegistry;
            _objectService = objectService;
        }

        public void Defend(Player player, float frameTime)
        {
            var projectiles = _objectService.GetAll<Projectile>();
            var modifiers = _gameDataRegistry.Get<PlayerModifiers>(player);

            if (modifiers.Polarity == 0)
            {
                AddShield(player);
                return;
            }

            foreach(var projectile in projectiles)
            {
                if (projectile.Owner == player.Id)
                    continue;

                Repulse(player, projectile, modifiers.Polarity);
            }
        }

        private void AddShield(Player player)
        {
            var inputData = _gameDataRegistry.Get<PlayerInputData>(player);
            if(inputData.Defend.IsPressed)
            {
                var effects = _gameDataRegistry.Get<PlayerEffects>(player);
                effects.Add(new RepulseEffect());
            }
        }

        private void Repulse(Player player, Projectile projectile, float polarity)
        {
            var distance = Vector2.Distance(player.Center, projectile.Center);
            if (distance > 250f)
                return;

            var xDist = player.Center.X - projectile.Center.X;
            var yDist = player.Center.Y - projectile.Center.Y;

            // Calculate the normalized direction vector from player to projectile
            var repulseDir = new Vector2(-xDist, -yDist);
            if (repulseDir.LengthSquared() > 0)
                repulseDir = Vector2.Normalize(repulseDir);
            else
                repulseDir = new Vector2(1, 0);

            var currentSpeed = projectile.Speed;
            var originalSpeed = currentSpeed.Length();

            // Add a scaled nudge in the repulsion direction
            float repulseScale = 547f; // This scale factor makes 1/-1 match your desired effect
            var newVelocity = currentSpeed + repulseDir * -polarity * repulseScale;
            if (newVelocity.LengthSquared() > 0)
                newVelocity = Vector2.Normalize(newVelocity) * originalSpeed;
            else
                newVelocity = currentSpeed;

            // Ownership logic (optional, as before)
            if ((newVelocity.X < 0 && projectile.Speed.X > 0) || (newVelocity.X > 0 && projectile.Speed.X < 0))
                projectile.Owner = player.Id;

            projectile.HurryTo(x: newVelocity.X, y: newVelocity.Y);
        }
    }
}
