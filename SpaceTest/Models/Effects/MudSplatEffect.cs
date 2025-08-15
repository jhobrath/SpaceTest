using GalagaFighter.Models.Players;
using GalagaFigther;
using Raylib_cs;
using System.Numerics;

namespace GalagaFighter.Models.Effects
{
    public class MudSplatEffect : PlayerEffect
    {
        protected override float Duration => 5f;
        public override string IconPath => "Sprites/Effects/mudsplaticon.png";

        private int _frame = 0;
        private float _rotation = 0f;
        private SpriteWrapper _spriteWrapper;
        private static Random _random = new Random();
        private Vector2 _positionCenter;

        public MudSplatEffect(Player player, Vector2 positionCenter)
            : base(player)
        {
            _frame = _random.Next(0, 3);
            _spriteWrapper = new SpriteWrapper(TextureLibrary.Get("Sprites/Effects/MudSplat.png"), 3, 100000f);
            _positionCenter = positionCenter;
            _rotation = (float)_random.NextDouble() * 360f;
        }

        public override void OnDraw()
        {
            var alpha = Math.Min(255f, _remainingTime * 255f);
            var color = new Color(255, 255, 255, alpha);

            _spriteWrapper.CurrentFrame = _frame;
            _spriteWrapper.DrawAnimated(_positionCenter, _rotation, 256f, 256f, color: color);
        }

        public bool IsNear(Rectangle rect)
        {
            return rect.X < _positionCenter.X + 128 && rect.X + rect.Width > _positionCenter.X - 128 &&
                   rect.Y < _positionCenter.Y + 128 && rect.Y + rect.Height > _positionCenter.Y - 128;
        }
    }
}