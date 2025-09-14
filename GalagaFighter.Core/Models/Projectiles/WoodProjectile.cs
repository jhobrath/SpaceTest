using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Static;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class WoodProjectile : Projectile
    {
        public static Vector2 _baseSize = new(150f, 15f);
        public static Vector2 _baseSpeed= new(7000f, 0f);

        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed;
        public override int BaseDamage => 0;
        public override Vector2 SpawnOffset => new(-40, 45);

        public bool _plankedThisFrame = false;
        private bool _plankedIntoPlayer;
        private bool _alreadyPlanked;
        private readonly int SpriteIndex = Game.Random.Next(1, 4);
        public bool IsPlanked => _plankedIntoPlayer;

        public WoodProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers)
            : base(controller, owner, new SpriteWrapper("Temp"), initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            SpriteIndex = 2;
            Sprite = new SpriteWrapper("Sprites/Projectiles/wooden_plank_" + SpriteIndex + ".png");
        }

        public override void Update(Game game)
        {
            if(_plankedThisFrame)
            {
                ScaleTo(y: 30f);
                Sprite = new SpriteWrapper("Sprites/Projectiles/wooden_plank_" + SpriteIndex + "_planked.png");
                SetDrawPriority(5);
                _plankedThisFrame = false;
                _plankedIntoPlayer = true;
            }

            base.Update(game);
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            _plankedThisFrame = true;
            return
            [
                new DefaultCollision(player.Id, initialPosition, initialSize, initialSpeed)
            ];
        }

        public override List<PlayerEffect> CreateEffects()
        {
            if (_alreadyPlanked)
                return [];

            _alreadyPlanked = true;
            return [new PlankedEffect()];
        }
    }
}