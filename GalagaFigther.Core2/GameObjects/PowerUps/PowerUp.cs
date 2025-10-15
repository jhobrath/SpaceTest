using GalagaFigther.Core2.Effects;
using GalagaFigther.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.GameObjects.PowerUps
{
    public abstract class PowerUp : GameObject
    {
        public Guid Owner { get; set; }

        private static Vector2 _defaultSize = new(87f, 41f);

        public PowerUp(Vector2 initialPosition, Vector2 initialSpeed, string texture)
            : base(initialPosition, _defaultSize, initialSpeed, new StillImageSprite(texture))
        {
        }

        public abstract List<PlayerEffect> CreateEffects(Player player);
    }
}
