using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Models.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Controllers
{
    public interface IGameDataRegistry
    {
        bool Has<T>(GameObject gameObject) where T : new();
        T Get<T>(GameObject gameObject) where T : new();
        T Get<T>() where T : new();
        void Set<T>(GameObject gameObject, T value);
    }

    public class GameDataRegistry : IGameDataRegistry
    {
        private readonly Dictionary<Type, Dictionary<Guid, object>> _instanceRegistry = [];

        private readonly Guid _gameId = Guid.NewGuid();

        public T Get<T>() where T : new()
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

        public T Get<T>(GameObject gameObject) where T : new()
        {
            if (!_instanceRegistry.ContainsKey(typeof(T)))
                _instanceRegistry.Add(typeof(T), []);

            var reg = _instanceRegistry[typeof(T)];
            if (!reg.ContainsKey(gameObject.Id))
                reg.Add(gameObject.Id, new T());

            return (T)reg[gameObject.Id];
        }

        public void Set<T>(GameObject gameObject, T? value)
        {
            if (value == null)
                throw new ArgumentException(null, nameof(value));

            if (!_instanceRegistry.ContainsKey(typeof(T)))
                _instanceRegistry.Add(typeof(T), []);

            var reg = _instanceRegistry[typeof(T)];
            if (!reg.ContainsKey(gameObject.Id))
                reg.Add(gameObject.Id, value);
        }
    }

    public abstract class ControllerBase<T> where T : GameObject
    {
        protected ControllerBase()
        {
        }

        public abstract void Update(T gameObject, float frameTime);
        public abstract void Draw(T gameObject, float frameTime);
    }
}
