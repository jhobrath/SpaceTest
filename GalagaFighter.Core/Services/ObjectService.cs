using GalagaFighter.Core.Models.Projectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IObjectService
    {
        void AddGameObject(GameObject gameObject);
        GameObject GetById(Guid owner);
        List<T> GetChildren<T>(Guid id) where T : GameObject;
        List<T> GetGameObjects<T>() where T : GameObject;
        List<GameObject> GetGameObjects();
        GameObject GetOwner(GameObject projectile);
        void RemoveGameObject(Guid id);
        void RemoveGameObject(GameObject gameObject);
        void Reset();
    }
    public class ObjectService : IObjectService
    {
        public Dictionary<Guid, GameObject> _gameObjects = [];

        public Dictionary<Type, List<GameObject>> _gameObjectsByType = [];

        public void AddGameObject(GameObject gameObject)
        {
            _gameObjects.Add(gameObject.Id, gameObject);
        }

        public void RemoveGameObject(GameObject gameObject) => RemoveGameObject(gameObject.Id);
        public void RemoveGameObject(Guid id)
        {
            _gameObjects.Remove(id);
        }

        public List<T> GetChildren<T>(Guid id)
            where T : GameObject
        {
            var output = new List<T>();
            foreach(var obj in _gameObjects.Values)
            {
                if(obj.Owner == id)
                {
                    if (obj is T typeSpecific)
                        output.Add(typeSpecific);
                }
            }

            return output;
        }

        public List<GameObject> GetGameObjects()
        {
            return _gameObjects.Values.ToList();
        }

        public List<T> GetGameObjects<T>()
            where T : GameObject
        {
            var output = new List<T>();
            foreach (var obj in _gameObjects.Values)
                if (obj is T typeSpecific)
                    output.Add(typeSpecific);

            return output;
        }

        public void Reset()
        {
            _gameObjects = [];
        }

        public GameObject GetOwner(GameObject gameObject)
        {
            return _gameObjects[gameObject.Owner];
        }

        public GameObject GetById(Guid owner)
        {
            return _gameObjects[owner];
        }
    }
}
