using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerManagerFactory
    {
        IPlayerEffectManager GetEffectManager(Player player);
        IPlayerEffectManager GetEffectManager(Guid playerId);
        IPlayerResourceManager GetResourceManager(Player player);
        IPlayerResourceManager GetResourceManager(Guid playerId);
    }

    public class PlayerEffectManagerFactory : IPlayerManagerFactory
    {
        // Store the mapping between players and their effect managers
        protected readonly Dictionary<Guid, IPlayerEffectManager> _playerEffectManagers = new();
        protected readonly Dictionary<Guid, IPlayerResourceManager> _playerResourceManagers = new();

        public IPlayerEffectManager GetEffectManager(Player player)
        {
            return GetEffectManager(player.Id);
        }

        public IPlayerEffectManager GetEffectManager(Guid playerId)
        {
            if (!_playerEffectManagers.TryGetValue(playerId, out var manager))
            {
                manager = new PlayerEffectManager();
                _playerEffectManagers[playerId] = manager;
            }
            return manager;
        }
        public IPlayerResourceManager GetResourceManager(Player player)
        {
            return GetResourceManager(player.Id);
        }

        public IPlayerResourceManager GetResourceManager(Guid playerId)
        {
            if (!_playerResourceManagers.TryGetValue(playerId, out var manager))
            {
                manager = new PlayerResourceManager();
                _playerResourceManagers[playerId] = manager;
            }
            return manager;
        }
    }
}