using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace GalagaFighter.Core.Models.Players
{
    public class Player : GameObject
    {
        public float Health { get; set; } = 100f;
        public bool IsPlayer1 { get; private set; }
        public List<PlayerEffect> Effects { get; set; } = new();
        public PlayerEffect SelectedProjectile { get; set; }

        //Since effects compile each frame, and can change size/rotation/color,
        //  we need a our base rect for when the effects wear off and a current-frame
        //  rect for rendering/collision
        public Rectangle CurrentFrameRect { get; set; }
        public float CurrentFrameRotation { get; set; } = 0f;
        public Color CurrentFrameColor { get; set; } = Color.White;
        public Vector2 CurrentFrameSpeed { get; set; } = new Vector2(0f,0f);
        public SpriteWrapper CurrentFrameSprite { get; set; }
        public float CurrentFrameDamage { get; set; } = 1f;

        private static readonly SpriteWrapper _defaultSprite = new SpriteWrapper(TextureService.Get("Sprites/Players/Player1.png"));
        private readonly IPlayerController _playerController;

        public Player(IPlayerController playerUpdater, Rectangle initialPosition, bool isPlayer1)
            : base(Game.Id, _defaultSprite, initialPosition.Position, initialPosition.Size, new Vector2(0,20f))
        {
            _playerController = playerUpdater;

            IsPlayer1 = isPlayer1;
            CurrentFrameRect = Rect;
            CurrentFrameSprite = Sprite;
            Effects.Add(new DefaultShootEffect());
            SelectedProjectile = Effects[0];
        }

        public override void Update(Game game)
        {
            _playerController.Update(game, this);
        }

        public override void Draw()
        {
            CurrentFrameSprite.Draw(Center, CurrentFrameRotation, CurrentFrameRect.Width, CurrentFrameRect.Height, CurrentFrameColor);
        }
    }
}
