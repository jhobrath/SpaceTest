using GalagaFighter.Core.Handlers.Players;
using GalagaFighter.Core.Models.Players;
using System;
using System.Collections.Generic;

namespace GalagaFighter.Core.Services
{
    public interface IPlayerEffectManagerFactory
    {
        IPlayerEffectManager GetEffectManager(Player player);
        IPlayerEffectManager GetEffectManager(Guid playerId);
    }

    public class PlayerEffectManagerFactory : IPlayerEffectManagerFactory
    {
        // Store the mapping between players and their effect managers
        protected readonly Dictionary<Guid, IPlayerEffectManager> _playerEffectManagers = new();

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
    }
}