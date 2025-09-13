using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Numerics;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class BeamProjectile : Projectile
    {
        public static Vector2 _baseSize => new(10f, 40f);
        public static Vector2 _baseSpeed => new(0f, 0f);
        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed;
        public override int BaseDamage => _baseDamage;
        public override bool DamageOverTime => true;
        public override Vector2 SpawnOffset => new(0, 0);

        private int _baseDamage = 2;
        private float _damageTimer = 0.1f;
        private const float _damageInterval = 0.1f;
        private Player _owner;
        private float _beamWidth = 10f;
        private const float _beamGrowthSpeed = 8000f;
        private float _originalX;

        private float _pulseLoop = 1f;
        private float _beamOpacity;

        private float _maxWidth;
        private float _maxLifetime = .15f;

        public BeamProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers, Color? color)
            : base(controller, owner, GetSprite(), new Vector2(initialPosition.X, initialPosition.Y - _baseSize.Y/2), _baseSize, _baseSpeed, modifiers)
        {
            _owner = owner;
            _originalX = Position.X;
            AudioService.PlayShootSound();
            SetDrawPriority(2);
            Color = Color.Red;

            _maxWidth = _owner.IsPlayer1
                ? Game.Width - (Rect.X)*1.25f
                : Game.Width - (Game.Width - Rect.X)*1.25f;
        }

        public void Reset()
        {
            _maxLifetime = Lifetime + .15f;
        }

        private static SpriteWrapper GetSprite()
        {
            return new SpriteWrapper("MissingSprites");
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            AudioService.PlayHitSound();
            return [new DefaultCollision(player.Id, initialPosition, initialSize, initialSpeed)];
        }

        public override void Update(Game game)
        {
            var frameTime = Raylib.GetFrameTime();

            _beamOpacity = .3f + MathF.Abs(.25f - .5f * (Lifetime % _pulseLoop));

            if(_beamWidth < _maxWidth)
            { 
                _beamWidth += _beamGrowthSpeed * frameTime;
                ScaleTo(x: _beamWidth, y: Rect.Height);
            }

            if (_owner.IsPlayer1)
            {
                MoveTo(x: _originalX);
            }
            else
            {
                MoveTo(x: _originalX - Rect.Width);
            }

            if (Lifetime > _maxLifetime)
                IsActive = false;

            base.Update(game);
        }

        public override void Draw()
        {

            var circleMaxRadius = Rect.Height/2;
            var circleRadius = MathF.Min(1, Lifetime / .5f);
            var rect = new Rectangle(Position.X + circleMaxRadius*(_owner.IsPlayer1 ? 1f : -1f), Position.Y, Rect.Width, Rect.Height);
            var startingColor = (_owner.IsPlayer1 ? Color.SkyBlue : Color.DarkBlue).ApplyAlpha(_beamOpacity);
            var endingColor = (_owner.IsPlayer1 ? Color.DarkBlue : Color.SkyBlue).ApplyAlpha(_beamOpacity);

            if (Lifetime > .5f)
                DrawSupplementaryBeam();

            var circleX = _owner.IsPlayer1
                ? Position.X + circleRadius * circleMaxRadius
                : Rect.X + Rect.Width -  circleRadius * circleMaxRadius;
            Raylib.DrawCircleSector(new Vector2(circleX, rect.Y + rect.Height / 2), circleRadius* circleMaxRadius, _owner.IsPlayer1 ? 90 : -90, _owner.IsPlayer1 ? 270 : 90, 1, _owner.IsPlayer1 ?  startingColor : endingColor);

            if (Lifetime < .5f)
                return;

            for(var i = 0; i < 4;i++)
            {
                var area = rect.Width / 4;
                var lowestX = rect.X + i * area;
                var highestX = rect.X + area * (i + 1);
                var diff = (highestX - lowestX) * (Lifetime % .25f)/.25f;
                var x = _owner.IsPlayer1 ? lowestX + diff : highestX - diff;
                var rect2 = new Rectangle(new Vector2(x, Rect.Position.Y), 50, Rect.Height);
                var startColor2 = _owner.IsPlayer1 ? Color.DarkBlue.ApplyAlpha(0f) : Color.DarkBlue;
                var endColor2 = _owner.IsPlayer1 ? Color.DarkBlue : Color.DarkBlue.ApplyAlpha(0f);

                Raylib.DrawRectangleGradientEx(rect2, startColor2, startColor2, endColor2, endColor2);
            }

            Raylib.DrawRectangleGradientEx(rect, startingColor, startingColor, endingColor, endingColor);
        }

        private void DrawSupplementaryBeam()
        {
            var startPoint = _owner.IsPlayer1
                ? new Vector2( Rect.X, Center.Y)
                : new Vector2(Rect.X + Rect.Width-90, Center.Y);

            var endPoint = _owner.IsPlayer1
                ? new Vector2(Rect.X + Rect.Width, Center.Y)
                : new Vector2(Rect.X+60, Center.Y);
            
            var numberOfLines = Math.Min(1, Lifetime / 4f) * 80;

            // Create the illusion of an energy beam by drawing multiple, randomized lines
            for (int i = 0; i < 20 + numberOfLines; i++)
            {
                // Generate random offsets for the beam's start and end points
                float startOffsetX = 30 + Game.Random.NextSingle() * 60 - 20;
                float startOffsetY = Game.Random.NextSingle() * 40 - 20;
                float endOffsetX = Game.Random.NextSingle() * 60 - 30 - 30;
                float endOffsetY = Game.Random.NextSingle() * 90 - 45;

                // Adjust the thickness for variation
                float lineThickness = Game.Random.NextSingle() * 5 + 1;

                // Create a transparent, brighter version of the color for the core
                var beamColor = Color.White.ApplyBlue(Game.Random.NextSingle());
                Color transparentColor = Raylib.ColorAlpha(beamColor, Game.Random.NextSingle() * 0.7f);

                // Draw the line with randomized points and thickness
                Raylib.DrawLineEx(
                    new Vector2(startPoint.X + startOffsetX, startPoint.Y + startOffsetY),
                    new Vector2(endPoint.X + endOffsetX, endPoint.Y + endOffsetY),
                    lineThickness,
                    transparentColor
                );
            }
        }

        public override bool? IsColliding(Player player)
        {
            if (Lifetime < .5f)
                return false;

            var dist = _owner.Center.Y - player.Center.Y;

            if (Math.Abs(dist) < player.Rect.Height/2f)
                return true;

            return false;
        }

        private float _lastEffect = 0f;
        public override List<PlayerEffect> CreateEffects()
        {
            if (Lifetime - _lastEffect > .5f)
            {
                _lastEffect = Lifetime;
                return [new RadiationEffect()];
            }
            return [];
        }
    }
}
