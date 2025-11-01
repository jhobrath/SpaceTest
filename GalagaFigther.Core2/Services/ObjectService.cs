using GalagaFighter.Core2.GameObjects;
using GalagaFighter.Core2.GameObjects.Guns;
using GalagaFighter.Core2.GameObjects.Turrets;
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
        IEnumerable<GameObject> GetChildren(GameObject parent);
        IEnumerable<T> GetChildren<T>(GameObject parent) where T : GameObject;
        Player GetPlayer(GameObject gun);
        int Count();
        Player GetOpponent(Player player);
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
            {
                if (!item.Value.IsActive)
                {
                    toRemove.Add(item.Key);

                    var children = GetChildren(item.Value);
                    foreach (var child in children)
                        toRemove.Add(item.Key);
                }
            }

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

        public IEnumerable<GameObject> GetChildren(GameObject gameObject)
        {
            return [.. Values.Where(x => x.Owner == gameObject.Id)];
        }

        public IEnumerable<T> GetChildren<T>(GameObject gameObject) 
            where T : GameObject
        {
            return [.. Values.Where(x => x.Owner == gameObject.Id).OfType<T>().Cast<T>()];
        }

        public Player GetPlayer(GameObject gameObject)
        {
            var players = new List<Guid>([Game.Player1Id, Game.Player2Id]);
            while (true)
            {
                if (players.Contains(gameObject.Id))
                    return (Player)gameObject;

                if (gameObject.Owner == Game.Id)
                    throw new ArgumentException(null, nameof(gameObject));

                gameObject = this[gameObject.Owner];
            }
        }

        public void Remove(GameObject gameObject)
        {
            Remove(gameObject.Id);
        }

        public int Count()
        {
            return Keys.Count();
        }

        public Player GetOpponent(Player player)
        {
            return GetAll<Player>().Single(x => x.Id != player.Id);
        }
    }
}
