using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IEventService
    {
        Guid Subscribe<T>(Action<T> listener) where T : EventArgs;
        void Unsubscribe<T>(Guid subscriptionId) where T : EventArgs;
        void Publish<T>(T eventArgs) where T : EventArgs;
    }

    public class EventService : IEventService
    {
        private static readonly Dictionary<Type, Dictionary<Guid, Action<object>>> _events = new();

        public Guid Subscribe<T>(Action<T> listener) where T : EventArgs
        {
            var type = typeof(T);
            if (!_events.ContainsKey(type))
            {
                _events[type] = new Dictionary<Guid, Action<object>>();
            }
            var id = Guid.NewGuid();
            _events[type][id] = e => listener((T)e);
            return id;
        }

        public void Unsubscribe<T>(Guid subscriptionId) where T : EventArgs
        {
            var type = typeof(T);
            if (_events.ContainsKey(type))
            {
                _events[type].Remove(subscriptionId);
                if (_events[type].Count == 0)
                    _events.Remove(type);
            }
        }

        public void Publish<T>(T eventArgs) where T : EventArgs
        {
            var type = typeof(T);
            if (_events.ContainsKey(type))
            {
                foreach (var action in _events[type].Values.ToList())
                {
                    action(eventArgs);
                }
            }
        }
    }
}
