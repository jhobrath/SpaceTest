using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.PowerUps
{
    public abstract class PowerUp : GameObject
    {
        private IPowerUpController _powerUpUpdater;

        public PowerUp(IPowerUpController powerUpUpdater, Guid owner, string texture, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
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
