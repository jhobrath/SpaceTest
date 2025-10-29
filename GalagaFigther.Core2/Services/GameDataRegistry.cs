using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.PowerUps;
using GalagaFighter.Core2.GameObjects.Projectiles;
using GalagaFighter.Core2.GameObjects.Turrets;
using GalagaFighter.Core2.Models;
using GalagaFighter.Core2.Models.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IGameDataRegistry : IClearable
    {
        T Get<T>() where T : class, IGameData, new();

        TData Get<TData>(Player player) where TData : class, IGameObjectData<Player>, new();
        TData Get<TData>(Projectile projectile) where TData : class, IGameObjectData<Projectile>, new();
        TData Get<TData>(PowerUp powerUp) where TData : class, IGameObjectData<PowerUp>, new();
        TData Get<TData>(Turret turret) where TData : class, IGameObjectData<Turret>, new();
        TData Get<TData>(ParticleEmitter emitter) where TData : class, IGameObjectData<ParticleEmitter>, new();
        TData Get<TData>(Gun gun) where TData : class, IGameObjectData<Gun>, new();

        bool Has<T>(GameObject gameObject) where T : class, new();
        void Remove(Guid id);

        void Set<TData>(Player player, TData data) where TData : class, IGameObjectData<Player>, new();
        void Set<TData>(Projectile projectile, TData data) where TData : class, IGameObjectData<Projectile>, new();
        void Set<TData>(PowerUp powerUp, TData data) where TData : class, IGameObjectData<PowerUp>, new();
        void Set<TData>(Turret turret, TData data) where TData : class, IGameObjectData<Turret>, new();
        void Set<TData>(ParticleEmitter emitter, TData data) where TData : class, IGameObjectData<ParticleEmitter>, new();
        void Set<TData>(Gun gun, TData data) where TData : class, IGameObjectData<Gun>, new();
    }

    public class GameDataRegistry : IGameDataRegistry
    {
        private readonly Dictionary<Type, Dictionary<Guid, object>> _instanceRegistry = [];

        private readonly Guid _gameId = Guid.NewGuid();

        public void Clear()
        {
            _instanceRegistry.Clear();
        }

        public T Get<T>() where T : class, IGameData, new()
        {
            if (!_instanceRegistry.ContainsKey(typeof(T)))
                _instanceRegistry.Add(typeof(T), []);

            var reg = _instanceRegistry[typeof(T)];
            if (!reg.ContainsKey(_gameId))
                reg.Add(_gameId, new T());

            return (T)reg[_gameId];
        }

        public bool Has<T>(GameObject gameObject) where T : class, new()
        {
            if (!_instanceRegistry.ContainsKey(typeof(T)))
                return false;

            var reg = _instanceRegistry[typeof(T)];
            if (!reg.ContainsKey(gameObject.Id))
                return false;

            return true;
        }

        private TData Get<TData, TGameObject>(TGameObject gameObject) 
            where TData : class, IGameObjectData<TGameObject>, new()
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

        public TData Get<TData>(Player player) where TData : class, IGameObjectData<Player>,new() => Get<TData, Player>(player);
        public TData Get<TData>(Projectile projectile) where TData : class, IGameObjectData<Projectile>,new() => Get<TData, Projectile>(projectile);
        public TData Get<TData>(PowerUp powerUp) where TData : class, IGameObjectData<PowerUp>,new() => Get<TData, PowerUp>(powerUp);
        public TData Get<TData>(Turret turret) where TData : class, IGameObjectData<Turret>,new() => Get<TData, Turret>(turret);
        public TData Get<TData>(ParticleEmitter emitter) where TData : class, IGameObjectData<ParticleEmitter>,new() => Get<TData, ParticleEmitter>(emitter);
        public TData Get<TData>(Gun gun) where TData : class, IGameObjectData<Gun>,new() => Get<TData, Gun>(gun);

        public void Set<TData>(Player player, TData data) where TData : class, IGameObjectData<Player>, new() => Set<TData, Player>(player, data);
        public void Set<TData>(Projectile projectile, TData data) where TData : class, IGameObjectData<Projectile>, new() => Set<TData, Projectile>(projectile, data);
        public void Set<TData>(PowerUp powerUp, TData data) where TData : class, IGameObjectData<PowerUp>, new() => Set<TData, PowerUp>(powerUp, data);
        public void Set<TData>(Turret turret, TData data) where TData : class, IGameObjectData<Turret>, new() => Set<TData, Turret>(turret, data);
        public void Set<TData>(ParticleEmitter emitter, TData data) where TData : class, IGameObjectData<ParticleEmitter>, new() => Set<TData, ParticleEmitter>(emitter, data);
        public void Set<TData>(Gun gun, TData data) where TData : class, IGameObjectData<Gun>, new() => Set<TData, Gun>(gun, data);

        public void Remove(Guid id) 
        {
            foreach (var reg in _instanceRegistry)
                reg.Value.Remove(id);
        }

    }
}
