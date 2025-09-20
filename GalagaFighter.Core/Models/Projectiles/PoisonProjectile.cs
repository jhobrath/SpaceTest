using GalagaFighter.Core.Controllers;
using GalagaFighter.Core.Models.Effects;
using GalagaFighter.Core.Models.Effects.Statuses;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Models.Projectiles
{
    public class PoisonProjectile : Projectile
    {
        public static readonly Vector2 _baseSpeed = new(500f, 0f);
        public static readonly Vector2 _baseSize = new(100f, 100f);
        private Vector2 _originalPosition;

        public override Vector2 BaseSpeed => _baseSpeed;
        public override Vector2 BaseSize => _baseSize;
        public override int BaseDamage => 0;
        public override Vector2 SpawnOffset => Vector2.Zero;
        public override bool DamageOverTime => true;

        public override bool RotationDrivenBySpeed => false;

        // Animation variables (changed from constants for debugging flexibility)
        private readonly float _bubbleFormationTime = .35f;//.35f; // average value
        
        // Track the initial offset from owner center for movement following
        private Vector2 _initialOffsetFromOwnerCenter;
        private readonly Player _owner;
        
        // Bubble wobble animation for realistic floating motion
        private float _wobbleFrequency1 = .8f;  // average value
        private float _wobbleFrequency2 = 1.6f;  // average value 
        private float _wobbleAmplitude = 1.3f;   // average value

        private readonly SpriteWrapper _formationSprite;
        private readonly SpriteWrapper _travelSprite;
        private readonly SpriteWrapper _popSprite;
        private readonly ParticleEffect _poisonEffect;

        public PoisonProjectile(IProjectileController controller, Player owner, SpriteWrapper sprite, Vector2 initialPosition, PlayerProjectile modifiers) 
            : base(controller, owner, sprite, initialPosition, _baseSize, _baseSpeed, modifiers)
        {
            initialPosition = new Vector2(initialPosition.X 
                + (owner.IsPlayer1 ? 0 : -1)*Rect.Width 
                + (owner.IsPlayer1 ? -1 : 1)*25, initialPosition.Y - Rect.Size.Y/ 2);
            MoveTo(initialPosition.X, initialPosition.Y);
            _originalPosition = initialPosition;
            _owner = owner;

            // Store the initial offset from the owner's center so we can follow the ship's movement
            _initialOffsetFromOwnerCenter = initialPosition - owner.Center;
            
            // Keep the projectile stationary during bubble formation
            Hurry(0f, 0f); // Set speed to 0 during formation
            
            _poisonEffect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.Smoke);
            _poisonEffect.UseGravity = false;
            _poisonEffect.ParticleDrag = 0f;
            _poisonEffect.Shape = EmissionShape.Point;
            _poisonEffect.ParticleStartSize = 10f;
            _poisonEffect.ParticleEndSize = 70f;
            _poisonEffect.ParticleSpeed = Vector2.Zero;// new Vector2((_owner.IsPlayer1 ? 1 : -1.2f) * _baseSpeed.X / 2, -20f);
            //_poisonEffect.Offset = -Vector2.One*_poisonEffect.ParticleStartSize/2;
            _poisonEffect.Offset = new Vector2(-_poisonEffect.ParticleStartSize/2, 100000f); //Don't show yet
            _poisonEffect.ParticleStartColor = Color.DarkGreen;
            _poisonEffect.ParticleEndColor = Color.DarkGreen.ApplyAlpha(0f);
            ParticleEffects.Add(_poisonEffect);

            _formationSprite = SpriteGenerationService2.CreatePoisonFormationSprite();
            _travelSprite = SpriteGenerationService2.CreatePoisonTravelSprite();
            _popSprite = SpriteGenerationService2.CreatePoisonPopSprite();

            Modifiers.RotationOffsetIncrement = 50f;
        }

        private float _lifeTime = 0f;
        private bool _bubbleFormationComplete = false;
        private float _bubblePopTime;

        public override void Update(Game game)
        {
            _lifeTime += Raylib.GetFrameTime();

            if (_bubblePopTime > .5f)
                UpdatePopComplete();
            else if (_bubblePopTime > 0f)
                UpdatePop();
            else if (_bubbleFormationComplete)
                UpdateTravel();
            else
                UpdateFormation();

            base.Update(game);
        }

        private void UpdateFormation()
        {
            if (_lifeTime >= _bubbleFormationTime)
                Release();

            var newPosition = _owner.Center + _initialOffsetFromOwnerCenter;
            MoveTo(newPosition.X, newPosition.Y);

            Sprite = _formationSprite;
        }

        private void UpdateTravel()
        {
            Sprite = _travelSprite;
        }

        private void UpdatePop()
        {
            _bubblePopTime += Raylib.GetFrameTime();
            Sprite = _popSprite;
        }

        private void UpdatePopComplete()
        {
            if (Rect.Size.X < 200f)
            {
                Move(-50, -50);
                ScaleTo(200f, 200f);
                _poisonEffect.ParticleSpeed = new Vector2(_poisonEffect.ParticleSpeed.X - 30f, -50f);
            }

            if (!_bubbleFormationComplete)
                IsActive = false;

            Sprite = new SpriteWrapper("NoTexture");
        }

        public void Pop()
        {
            if (_bubblePopTime > 0)
                return;

            _bubblePopTime = Raylib.GetFrameTime();

            _poisonEffect.ParticleEndSize = 300f;
            _poisonEffect.EmissionRate *= 2;
            _poisonEffect.FollowRotation = true;
            //Modifiers.RotationOffsetIncrement = 5f;
            _poisonEffect.Offset += Vector2.One * -35;
            _poisonEffect.ParticleSpeed = new Vector2(Speed.X / 2.1f, -20 - (float)Game.Random.NextDouble() * 40f);
            _poisonEffect.ParticleSizeVariation = 100f;
        }

        private void Release()
        {
            _bubbleFormationComplete = true;

            // Set the speed to make the bubble travel
            var actualSpeed = new Vector2(
                BaseSpeed.X * (_owner.IsPlayer1 ? 1f : -1f),
                BaseSpeed.Y
            );
            HurryTo(actualSpeed.X, actualSpeed.Y);

            if (_owner.IsPlayer1)
                _poisonEffect.ParticleSpeed = new Vector2(Speed.X / 2.1f, -20f);
            else
                _poisonEffect.ParticleSpeed = new Vector2(Speed.X / 1.9f, -20f);

            _poisonEffect.Offset = -Vector2.One * _poisonEffect.ParticleStartSize / 2 + new Vector2(0, 5); ;
        }


        public override List<PlayerEffect> CreateEffects()
        {
            return [new PoisonedEffect()];
        }
    }
}
