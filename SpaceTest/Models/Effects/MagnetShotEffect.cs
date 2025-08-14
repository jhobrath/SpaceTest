using GalagaFighter;
using GalagaFighter.Models.Effects;
using GalagaFighter.Models.Players;
using GalagaFigther.Models.Projectiles;
using Raylib_cs;
using System.Numerics;

namespace GalagaFigther.Models.Effects
{
    public class MagnetShotEffect : PlayerEffect
    {
        private Vector2 _attractPosition;
        public override bool DisableShooting => true;
        public override string IconPath => "Sprites/Effects/magnetshot.png";

        private List<Projectile> _caughtProjectiles = new List<Projectile>();
        private Dictionary<Projectile, Vector2> _originalSpeeds = new Dictionary<Projectile, Vector2>();
        private static Random _random = new Random();

        private bool _shootDown = false;

        protected override float Duration => 5f;

        public MagnetShotEffect(Player player) : base(player)
        {
            _attractPosition = new Vector2(
                player.Rect.X + (player.IsPlayer1 ? player.Rect.Width + 50 : -50),
                player.Rect.Y + player.Rect.Height/2
            );
        }

        public override void OnStatsSwitch()
        {
            _shootDown = false;
            OnUpdate(0f);
        }

        public override void ModifyPlayerRendering(PlayerRendering playerRendering)
        {
            playerRendering.Texture = "Sprites/Players/MagnetShotShip.png";
        }

        public override void OnUpdate(float frameTime)
        {
            _attractPosition = new Vector2(
                Player.Rect.X + (Player.IsPlayer1 ? Player.Rect.Width + 50 : -50),
                Player.Rect.Y + Player.Rect.Height / 2
            );

            if(!Raylib.IsKeyDown(Player.GetShootKey()))
            {
                var fired = false;
                _caughtProjectiles.ForEach(p => {
                    p.Speed = new Vector2(Player.IsPlayer1 ? 75f : -75f, 10f - 20f * (float)_random.NextDouble());
                    p.Owner = Player;
                    fired = true && _shootDown;
                });
                foreach (var key in _originalSpeeds.Keys)
                { 
                    key.Speed = _originalSpeeds[key];
                    _originalSpeeds.Remove(key);
                }

                if (fired)
                {
                    Game.PlayExplosionConversionSound();
                    _shootDown = false;
                }
            }

            base.OnUpdate(frameTime);
        }

        public override void OnShoot(Game game)
        {
            if(!_shootDown)
            {
                Game.PlayMagnetSound();
                _shootDown = true;
            }

            foreach (var gameObject in game.GetGameObjects())
            {
                if(gameObject is Projectile proj)
                {
                    if (!_originalSpeeds.ContainsKey(proj))
                        _originalSpeeds[proj] = proj.Speed;

                    var newGameX = (_attractPosition.X - gameObject.Rect.X) / 50;
                    var newGameY = (_attractPosition.Y - gameObject.Rect.Y) / 50;

                    var newSpeed = new Vector2(newGameX, newGameY);
                    proj.Speed = newSpeed;

                    if (!_caughtProjectiles.Contains(proj) && newSpeed.X < 10)
                    { 
                        _caughtProjectiles.Add(proj);
                        _originalSpeeds.Remove(proj);
                    }
                }
            }

            base.OnShoot(game);
        }
    }
}
