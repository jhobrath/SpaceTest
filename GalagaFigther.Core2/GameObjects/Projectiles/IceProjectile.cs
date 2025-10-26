using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Effects.Statuses;
using GalagaFighter.Core2.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects.Projectiles
{
    public class IceProjectile : Projectile
    {
        private static Vector2 _baseSpeed => new(200f, 0f);
        private static Vector2 _baseSize => new(95f, 42f);

        private static Vector2[] _bounds = new Vector2[]
        {
            new(.04f, .04f),
            new(.91f, .465f),
            new(.04f, .96f)
        };

        public IceProjectile(Guid owner) 
            : base(owner, Vector2.Zero, _baseSize, _baseSpeed, GetSprite())
        {
            Bounds = _bounds;
        }

        private static SpriteBase GetSprite()
        {
            return new NonRepeatingAnimatedImageSprite("Sprites/Projectiles/ice.png", 6, 570 / 6, 42, .0625f);
        }

        public override List<PlayerEffect> CreateEffects(Player player)
        {
            return [new FrozenEffect()];
        }
    }
}
