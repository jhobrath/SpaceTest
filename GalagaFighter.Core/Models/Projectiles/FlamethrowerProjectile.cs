using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Collisions;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class FlamethrowerProjectile : Projectile
    {
        private static Vector2 _baseSize => new(70, 70);
        private static Vector2 _baseSpeed => new(800f, 0f); // Beam is stationary, only animates
        public override Vector2 BaseSize => _baseSize;
        public override Vector2 BaseSpeed => _baseSpeed;
        public override int BaseDamage => _baseDamage;
        public override Vector2 SpawnOffset => new(-50, 0);

        private int _baseDamage = 0;

        private float _damageTimer = .3f;
        private ParticleEffect _particleEffect;

        public FlamethrowerProjectile(IProjectileController controller, Player owner, Vector2 initialPosition, PlayerProjectile modifiers, Color? color)
            : base(controller, owner, GetSprite(), new Vector2(initialPosition.X, initialPosition.Y - _baseSize.Y/2)   , _baseSize, _baseSpeed, modifiers)
        {
            AudioService.PlayShootSound();

            SetDrawPriority(-2);

            _particleEffect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.FlamethrowerBeam);
            _particleEffect.UseGravity = false;
            _particleEffect.ParticleStartSize = 70f;
            _particleEffect.ParticleSpeed =  new Vector2(200f*(owner.IsPlayer1 ? 1 : -1),0f);
            _particleEffect.ParticleStartColor = Color.Orange;
            _particleEffect.ParticleEndColor = Color.Yellow;
            _particleEffect.ParticleColorVariation = 50f;
            _particleEffect.ParticleLifetime = 1f;
            _particleEffect.EmissionRate *= 1f;
            _particleEffect.EmissionRadius *= 2f;
            _particleEffect.FollowRotation = true;
            _particleEffect.ParticleSizeVariation = 20f;
            _particleEffect.Offset = new(_particleEffect.ParticleStartSize / 2 + (owner.IsPlayer1 ? -70 : 60), (owner.IsPlayer1 ? -1 : 1) * _particleEffect.ParticleStartSize / 2);// Vector2.Zero;// new Vector2((owner.IsPlayer1 ? -1 : 1)*_particleEffect.ParticleStartSize / 2 + 50f, (owner.IsPlayer1 ? -1 : 1) * _particleEffect.ParticleStartSize / 2);
            ParticleEffects.Add(_particleEffect);
        }

        private static SpriteWrapper GetSprite()
        {
            return new SpriteWrapper("MissingSprites");
        }

        public override List<Collision> CreateCollisions(Player player, Vector2 initialPosition, Vector2 initialSize, Vector2 initialSpeed)
        {
            AudioService.PlayHitSound();
            return
            [
                new DefaultCollision(player.Id, initialPosition, initialSize, initialSpeed)
            ];
        }

        public override void Update(Game game)
        {
    
            _damageTimer -= Raylib.GetFrameTime();
            if (_damageTimer <= 0f)
            {
                _damageTimer = .1f;
                if(Game.Random.NextDouble() < .5)
                    _baseDamage = 1;
            }
            else
                _baseDamage = 0;

            base.Update(game);
        }
    }
}
