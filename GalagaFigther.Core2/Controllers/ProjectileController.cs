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
        private readonly Dictionary<Type, IProjectileBehavior> _behaviorInstances;

        public ProjectileController(IEnumerable<IProjectileBehavior> behaviors, IGameDataRegistry gameDataRegistry)
        {
            _behaviorInstances = behaviors.ToDictionary(b => b.GetType());
            _gameDataRegistry = gameDataRegistry;
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
            // Register all state models for this projectile
            foreach (var state in projectile.StateModels)
            {
                _gameDataRegistry.Set(projectile, state);
            }

            foreach (var behaviorType in projectile.Behaviors)
            {
                if (_behaviorInstances.TryGetValue(behaviorType, out var behavior))
                {
                    behavior.Update(projectile, frameTime);
                }
            }
        }
    }
}
