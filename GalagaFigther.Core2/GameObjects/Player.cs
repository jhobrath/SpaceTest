using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.GameObjects
{
    public class Player : GameObject
    {
        public float Health { get; set; } = 100f;

        public PlayerEffect DefensiveAugment { get; set; }

        public override Vector2 Drag => Vector2.One*6;

        private readonly Vector2[] _bounds = new Vector2[]{
            new(0.045f, 0.685f),  // Left wing: 4.5% in, 68.5% down
            new(0.955f, 0.685f),  // Right wing: 95.5% in, 68.5% down
            new(0.5f, 0.08f)      // Ship tip: 50% in, 8% down
        };

        public Player(Vector2 position, Vector2 size, Vector2 speed, SpriteBase sprite) 
            : base(Game.Id, position, size, speed, sprite)
        {
            Bounds = _bounds;
        }
    }
}
