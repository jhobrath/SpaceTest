using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Models.Players;
using GalagaFighter.Core.Models.Projectiles;
using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.CPU
{
    public interface IGameStateCollector
    {
        GameState Collect();
    }
    public class GameStateCollector
    {
        private readonly IObjectService _objectService;
        private readonly IPlayerManagerFactory _playerManagerFactory;

        private Player? _player1;
        private Player? _player2;
        private Dictionary<Guid, IPlayerResourceManager>? _resourceManagers;
        private bool _initialized = false;

        public GameStateCollector(IObjectService objectService, IPlayerManagerFactory playerManagerFactory)
        {
            _objectService = objectService;
            _playerManagerFactory = playerManagerFactory;
        }

        public GameState Collect()
        {
            if (!_initialized)
                Initialize();

            var state = new GameState()
            {
                Player1 = GetPlayerState(_player1!),
                Player2 = GetPlayerState(_player2!),
                Player1Projectiles = GetPlayerProjectiles(_player1!, _player2!),
                Player2Projectiles = GetPlayerProjectiles(_player2!, _player1!)
            };

            return state;
        }

        private List<GameStateForProjectile> GetPlayerProjectiles(Player player, Player opponent)
        {
            var projectiles = _objectService.GetChildren<Projectile>(player.Id);

            var closeProjectiles = projectiles
                .OrderByDescending(x => Vector2.Distance(x.Center, opponent.Center))
                .ToList();

            var output = Enumerable.Range(0, 4)
                .Select(x => closeProjectiles.Count > x ? closeProjectiles[x] : null)
                .Select(GetProjectileState).ToList();

            return output;
        }

        private GameStateForProjectile GetProjectileState(Projectile? projectile)
        {
            var state = new GameStateForProjectile() { };
            state.SpeedX = projectile?.Speed.Y ?? -1;
            state.SpeedY = projectile?.Speed.X ?? -1;
            state.PositionX = projectile?.Center.X ?? -1;
            state.PositionY = projectile?.Center.Y ?? -1;
            state.Damage = (projectile?.Modifiers?.DamageMultiplier * projectile?.BaseDamage) ?? -1;
            return state;
        }

        private GameStateForPlayer GetPlayerState(Player player)
        {
            return new GameStateForPlayer
            {
                Health = player.Health,
                PositionX = player.Center.X,
                PositionY = player.Center.Y,
                Resource = _resourceManagers[player.Id].ShieldMeter
            };
        }

        private void Initialize()
        {
            var players = _objectService.GetGameObjects<Player>();
            _resourceManagers = players
                .Select(x => new { Player = x, ResourceManager = _playerManagerFactory.GetResourceManager(x.Id) })
                .ToDictionary(x => x.Player.Id, x => x.ResourceManager);

            _player1 = players.Single(x => x.IsPlayer1);
            _player2 = players.Single(x => !x.IsPlayer1);

            _initialized = true;
        }
    }
}
