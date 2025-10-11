using GalagaFigther.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFigther.Core2.Services
{
    public interface IObjectService
    {
        IEnumerable<GameObject> GetAll();
        IEnumerable<T> GetAll<T>() where T : GameObject;
        GameObject Get(Guid id);
        void Add(GameObject gameObject);
        void Remove(GameObject gameObject);
    }

    public class ObjectService : Dictionary<Guid, GameObject>, IObjectService, IDictionary<Guid, GameObject>
    {
        public void Add(GameObject gameObject)
        {
            Add(gameObject.Id, gameObject);
        }

        public GameObject Get(Guid id)
        {
            return this[id];
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
