using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IObjectService : IClearable
    {
        IEnumerable<GameObject> GetAll();
        IEnumerable<T> GetAll<T>() where T : GameObject;
        GameObject Get(Guid id);
        T Get<T>(Guid id) where T : GameObject;
        void Add(GameObject gameObject);
        void Remove(GameObject gameObject);
        void CleanUp();
    }

    public class ObjectService : Dictionary<Guid, GameObject>, IObjectService, IDictionary<Guid, GameObject>
    {
        private readonly IGameDataRegistry _gameDataRegistry;

        public ObjectService(IGameDataRegistry gameDataRegistry)
        {
            _gameDataRegistry = gameDataRegistry;
        }

        public void Add(GameObject gameObject)
        {
            Add(gameObject.Id, gameObject);
        }

        public void CleanUp()
        {
            var toRemove = new List<Guid>();
            foreach (var item in this)
                if (!item.Value.IsActive)
                    toRemove.Add(item.Key);

            foreach (var key in toRemove)
            {
                Remove(key);
                _gameDataRegistry.Remove(key);
            }
        }

        public GameObject Get(Guid id)
        {
            return this[id];
        }

        public T Get<T>(Guid id) where T : GameObject
        {
            return (T)this[id];
        }

        public IEnumerable<GameObject> GetAll()
        {
            return [..Values];
        }

        public IEnumerable<T> GetAll<T>() where T : GameObject
        {
            return [.. Values.OfType<T>()];
        }

        public void Remove(GameObject gameObject)
        {
            Remove(gameObject.Id);
        }
    }
}
