using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IGameDataRegistry
    {
        T Get<T>() where T : IGameData, new();
        TData Get<TData>(Player player) where TData : IGameObjectData<Player>, new();
        TData Get<TData>(Projectile projectile) where TData : IGameObjectData<Projectile>, new();
        TData Get<TData>(PowerUp powerUp) where TData : IGameObjectData<PowerUp>, new();
        bool Has<T>(GameObject gameObject) where T : new();
        void Set<TData>(Player player, TData data) where TData : IGameObjectData<Player>, new();
        void Set<TData>(Projectile projectile, TData data) where TData : IGameObjectData<Projectile>, new();
        void Set<TData>(PowerUp powerUp, TData data) where TData : IGameObjectData<PowerUp>, new();
    }

    public class GameDataRegistry : IGameDataRegistry
    {
        private readonly Dictionary<Type, Dictionary<Guid, object>> _instanceRegistry = [];

        private readonly Guid _gameId = Guid.NewGuid();

        public T Get<T>() where T : IGameData, new()
        {
            if (!_instanceRegistry.ContainsKey(typeof(T)))
                _instanceRegistry.Add(typeof(T), []);

            var reg = _instanceRegistry[typeof(T)];
            if (!reg.ContainsKey(_gameId))
                reg.Add(_gameId, new T());

            return (T)reg[_gameId];
        }

        public bool Has<T>(GameObject gameObject) where T : new()
        {
            if (!_instanceRegistry.ContainsKey(typeof(T)))
                return false;

            var reg = _instanceRegistry[typeof(T)];
            if (!reg.ContainsKey(gameObject.Id))
                return false;

            return true;
        }

        private TData Get<TData, TGameObject>(TGameObject gameObject) 
            where TData : IGameObjectData<TGameObject>, new()
            where TGameObject : GameObject
        {
            if (!_instanceRegistry.ContainsKey(typeof(TData)))
                _instanceRegistry.Add(typeof(TData), []);

            var reg = _instanceRegistry[typeof(TData)];
            if (!reg.ContainsKey(gameObject.Id))
                reg.Add(gameObject.Id, new TData());

            return (TData)reg[gameObject.Id];
        }

        private void Set<TData, TGameObject>(TGameObject gameObject, TData data)
            where TData : IGameObjectData<TGameObject>, new()
            where TGameObject : GameObject
        {
            if (data == null)
                throw new ArgumentException(null, nameof(data));

            if (!_instanceRegistry.ContainsKey(typeof(TData)))
                _instanceRegistry.Add(typeof(TData), []);

            var reg = _instanceRegistry[typeof(TData)];
            if (!reg.ContainsKey(gameObject.Id))
                reg.Add(gameObject.Id, data);
            else
                reg[gameObject.Id] = data;
        }

        public TData Get<TData>(Player player) where TData : IGameObjectData<Player>,new() => Get<TData, Player>(player);
        public TData Get<TData>(Projectile projectile) where TData : IGameObjectData<Projectile>,new() => Get<TData, Projectile>(projectile);
        public TData Get<TData>(PowerUp powerUp) where TData : IGameObjectData<PowerUp>,new() => Get<TData, PowerUp>(powerUp);

        public void Set<TData>(Player player, TData data) where TData : IGameObjectData<Player>, new() => Set<TData, Player>(player, data);
        public void Set<TData>(Projectile projectile, TData data) where TData : IGameObjectData<Projectile>, new() => Set<TData, Projectile>(projectile, data);
        public void Set<TData>(PowerUp powerUp, TData data) where TData : IGameObjectData<PowerUp>, new() => Set<TData, PowerUp>(powerUp, data);
    }
}
