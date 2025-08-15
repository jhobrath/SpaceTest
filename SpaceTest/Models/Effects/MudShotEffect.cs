using GalagaFighter.Models.Players;
using GalagaFigther;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using SpaceTest.Models.Projectiles;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class MudShotEffect : ProjectileEffect
    {
        protected override float Duration => 5.0f;

        public MudShotEffect(Player player) : base(player)
        {
        }

        protected override int ProjectileWidth => 150;
        protected override int ProjectileHeight => 64;
        protected override Projectile Spawn(Rectangle rect, Vector2 speed) => new MudProjectile(rect, speed, Player, this);

        protected override Vector2 SpawnOffset => new Vector2(-75, 20);

        public float _frameCounter = 0f;
        public int _frame = 0;

        public override void OnUpdate(float frameTime)
        {
            _frameCounter += .25f;
            _frame = Convert.ToInt32(Math.Floor(_frameCounter));

            base.OnUpdate(frameTime);
        }

        protected override SpriteWrapper Texture => new SpriteWrapper(TextureLibrary.Get("Sprites/Players/MudShotShip.png"), 3, .12f);
        public override int? TextureFrame => _frame;
        public override string IconPath => "Sprites/Effects/Mudshot.png";

    }
}