using GalagaFigther.Core2.GameObjects;
using GalagaFigther.Core2.Models.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Controllers
{
    public interface IGameDataRegistry
    {
        T Get<T>(GameObject gameObject) where T : new();
        T Get<T>() where T : new();
    }

    public class GameDataRegistry : IGameDataRegistry
    {
        private readonly Dictionary<Type, Dictionary<Guid, object>> _instanceRegistry = [];

        private readonly Guid _gameId = Guid.NewGuid();

        public T Get<T>() where T : new()
        {
            if (!_instanceRegistry.ContainsKey(typeof(T)))
                _instanceRegistry.Add(typeof(T), new());

            var reg = _instanceRegistry[typeof(T)];
            if (!reg.ContainsKey(_gameId))
                reg.Add(_gameId, new T());

            return (T)reg[_gameId];
        }

        public T Get<T>(GameObject gameObject) where T : new()
        {
            if (!_instanceRegistry.ContainsKey(typeof(T)))
                _instanceRegistry.Add(typeof(T), new());

            var reg = _instanceRegistry[typeof(T)];
            if (!reg.ContainsKey(gameObject.Id))
                reg.Add(gameObject.Id, new T());

            return (T)reg[gameObject.Id];
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
