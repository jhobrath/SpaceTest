using GalagaFighter.Core.Models;
using GalagaFighter.Core.Models.Debris;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Services;
using GalagaFighter.Core.Static;
using Raylib_cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Handlers.Players
{
    public interface IPlayerTetherDeployer
    {
        void Deploy(Player player);
    }
    public class PlayerTetherDeployer : IPlayerTetherDeployer
    {
        private readonly IInputService _inputService;
        private readonly IObjectService _objectService;
        private readonly ParticleEffect _moveEffect;
        private readonly float _maxTetherDistance = 250f;

        private const float _minimumDeployChangeTime = 1f;
        private float _deployTime = _minimumDeployChangeTime;

        private const float _minimumDeadTetherTimeout = 5f;
        private float _deadTetherTime = 0f;


        private List<Tether> _tethers = [];
        private readonly Lazy<SpriteWrapper> _tetherFull;
        private readonly Lazy<SpriteWrapper> _tetherDamage1;
        private readonly Lazy<SpriteWrapper> _tetherDamage2;
        private readonly Lazy<SpriteWrapper> _tetherDamage3;

        public PlayerTetherDeployer(IInputService inputService, IObjectService objectService)
        {
            _inputService = inputService;
            _objectService = objectService;

            _moveEffect = ParticleEffectsLibrary.Get(ParticleEffectLibraryKeys.FireTrail);
            _moveEffect.ParticleStartColor = Color.SkyBlue;
            _moveEffect.ParticleEndColor = Color.DarkBlue.ApplyAlpha(0f);
            _moveEffect.ParticleStartSize = 10f;
            _moveEffect.ParticleEndSize = 1f;
            _moveEffect.Offset = new System.Numerics.Vector2(-10f, 0f);

            _tetherFull = new Lazy<SpriteWrapper>(() => new SpriteWrapper(@"Sprites\Debris\TetherShield.png"));
            _tetherDamage1 = new Lazy<SpriteWrapper>(() => new SpriteWrapper(@"Sprites\Debris\TetherShield_damage1.png"));
            _tetherDamage2 = new Lazy<SpriteWrapper>(() => new SpriteWrapper(@"Sprites\Debris\TetherShield_damage2.png"));
            _tetherDamage3 = new Lazy<SpriteWrapper>(() => new SpriteWrapper(@"Sprites\Debris\TetherShield_damage3.png"));
        }

        public void Deploy(Player player)
        {
            _deployTime += Raylib.GetFrameTime();
            _deadTetherTime += Raylib.GetFrameTime();

            if (_tethers.Count > 0 && _tethers[0].Health <= 0)
                _deadTetherTime = 0f;

            DeployTether(player);

            foreach (var tether in _tethers)
            {
                MoveTether(player, tether);
                DecelerateTether(tether);
                UpdateSprite(tether);
                DestroyTether(tether);
            }
        }

        private void DestroyTether(Tether tether)
        {
            if (tether.Health > 0f)
                return;

            tether.IsActive = false;
        }

        private void UpdateSprite(Tether tether)
        {
            if (tether.Health > 75f)
                tether.Sprite = _tetherFull.Value;
            else if (tether.Health > 50f)
                tether.Sprite = _tetherDamage1.Value;
            else if (tether.Health > 25f)
                tether.Sprite = _tetherDamage2.Value;
            else
                tether.Sprite = _tetherDamage3.Value;
        }

        private void DeployTether(Player player)
        {
            if (_deadTetherTime <= _minimumDeadTetherTimeout)
                return;

                var deploy = _inputService.GetDeploy(player.Id);
            if (deploy.IsPressed && _deployTime > _minimumDeployChangeTime)
            {
                if (_tethers.Count == 0)
                {
                    var tether = new Tether(player);
                    _tethers.Add(tether);
                    _objectService.AddGameObject(tether);
                }
                else
                {
                    _tethers[0].IsActive = false;
                    _tethers.Remove(_tethers[0]);
                }

                _deployTime = 0f;
            }
        }

        private static void DecelerateTether(Tether tether)
        {
            float deceleration = 1000f; // units per second^2
            float frameTime = Raylib.GetFrameTime();

            float currentSpeed = tether.Speed.Y;
            float decel = deceleration * frameTime;

            if (Math.Abs(currentSpeed) <= decel)
                currentSpeed = 0;
            else
                currentSpeed -= Math.Sign(currentSpeed) * decel;

            tether.HurryTo(y: currentSpeed);
        }

        private void MoveTether(Player player, Tether tether)
        {
            var vertDistance = tether.Center.Y - player.Center.Y;
            if (Math.Abs(vertDistance) <= _maxTetherDistance)
            {
                tether.RemoveParticleEffects(_moveEffect.Name);
                return;
            }

            if (tether.Center.Y < player.Center.Y)
            {
                tether.MoveTo(y: player.Rect.Y - _maxTetherDistance);
                tether.HurryTo(y: player.Speed.Y);
            }
            else
            {
                tether.MoveTo(y: player.Rect.Y + _maxTetherDistance);
                tether.HurryTo(y: player.Speed.Y);
            }

            tether.AddParticleEffect(_moveEffect);
        }
    }
}