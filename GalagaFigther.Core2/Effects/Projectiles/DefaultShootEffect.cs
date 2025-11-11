using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.Helpers;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Effects.Projectiles
{
    public class DefaultShootEffect : PlayerEffect
    {
        protected override float Duration => 0f;
        private readonly SpriteDecoration _moveDecoration = new SpriteDecoration(new StillImageSprite("Sprites/Ships/MainShip_Move.png")) { Key = "Move", MaintainRotation = true };

        public override void Apply(PlayerModifiers modifiers)
        {
            modifiers.Decorations.Create[this] = (GameObject g, PlayerModifiers m) => [_moveDecoration];
            modifiers.PlayerActions.Add(player =>
            {
                // Ensure at least one DefaultGun exists
                if (!player.Guns.Any(g => g is DefaultGun))
                    player.Guns.Add(new DefaultGun(player, false));
            });
        }
    }
}
