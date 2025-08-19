using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.Services
{
    public interface IEventService
    {
        void Subscribe<T>(Action<T> listener) where T : EventArgs;
        void Unsubscribe<T>(Action<T> listener) where T : EventArgs;
        void Publish<T>(T eventArgs) where T : EventArgs;
    }

    public class EventService : IEventService
    {
        private static readonly Dictionary<Type, List<Action<object>>> _events = new Dictionary<Type, List<Action<object>>>();

        public void Subscribe<T>(Action<T> listener) where T : EventArgs
        {
            var type = typeof(T);
            if (!_events.ContainsKey(type))
            {
                _events[type] = new List<Action<object>>();
            }
            _events[type].Add(e => listener((T)e));
        }

        public void Unsubscribe<T>(Action<T> listener) where T : EventArgs
        {
            var type = typeof(T);
            if (_events.ContainsKey(type))
            {
                _events.Remove(type);
            }
        }

        public void Publish<T>(T eventArgs) where T : EventArgs
        {
            var type = typeof(T);
            if (_events.ContainsKey(type))
            {
                foreach (var action in _events[type])
                {
                    action(eventArgs);
                }
            }
        }
    }
}
