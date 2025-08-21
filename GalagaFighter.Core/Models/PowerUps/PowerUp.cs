using GalagaFighter.Core.Behaviors.PowerUps;
using GalagaFighter.Core.Behaviors.PowerUps.Interfaces;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.PowerUps
{
    public abstract class PowerUp : GameObject
    {
        private IPowerUpUpdater _powerUpUpdater;

        public PowerUp(IPowerUpUpdater powerUpUpdater, Guid owner, string texture, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
            : base(owner, new SpriteWrapper(TextureService.Get(texture)), initialPosition, initialSize, initialSpeed)
        {
            _powerUpUpdater = powerUpUpdater;
        }

        public override void Update(Game game)
        {
            _powerUpUpdater.Update(game, this);
        }

        public override void Draw()
        {
            Sprite.Draw(Center, Rotation, Rect.Width, Rect.Height, Color);
        }

        public abstract List<PlayerEffect> CreateEffects();
    }
}
