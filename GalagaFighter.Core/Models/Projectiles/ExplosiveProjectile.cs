using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class ExplosiveProjectile : Projectile
    {
        private static Vector2 _baseSize => new(40f, 40f);
        private static Vector2 _baseSpeed => new(1020f, 0f);
        
        public override Vector2 BaseSpeed => _baseSize;
        public override Vector2 BaseSize => _baseSpeed;
        public override int BaseDamage => 25;

        public override Vector2 SpawnOffset => new Vector2(-50, 15);

        public ExplosiveProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, GetSprite(), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
        }

        public override void Update(Game game)
        {
            var timer = (float)Modifiers["ExplodeTimer"];
            if (timer <= 0)
                return;

            timer -= Raylib.GetFrameTime();

            if (timer > 0f)
                Modifiers["ExplodeTimer"] = timer;
            else { 
                Sprite = (SpriteWrapper)Modifiers["ExplodeSprite"];
                Modifiers.SizeMultiplier = 7f;
                Modifiers.SpeedMultiplier = .5f;
            }

            base.Update(game);
        }

        private static SpriteWrapper GetSprite()
        {
            var texture = TextureService.Get("Sprites/Projectiles/explosion.png"); //Never use frame 2
            return new SpriteWrapper(texture, 2, 1000f);
        }
    }
}
