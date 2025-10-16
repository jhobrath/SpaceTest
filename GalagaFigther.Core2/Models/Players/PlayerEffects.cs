using GalagaFighter.Core2.Effects;
using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Models.Players
{
    public class PlayerEffects : IList<PlayerEffect>, IGameObjectData<Player>
    {
        public bool RequireRerolling { get;set; }
        private readonly List<PlayerEffect> _effects = [];

        public PlayerEffect this[int index] { get => _effects[index]; set => _effects[index] = value; }
        public int Count => _effects.Count;
        public bool IsReadOnly => false;
        public void Add(PlayerEffect item) => _effects.Add(item);
        public void Clear() => _effects.Clear();
        public bool Contains(PlayerEffect item) => _effects.Contains(item);
        public void CopyTo(PlayerEffect[] array, int arrayIndex) => _effects.CopyTo(array, arrayIndex);
        public IEnumerator<PlayerEffect> GetEnumerator() => _effects.GetEnumerator();
        public int IndexOf(PlayerEffect item) => _effects.IndexOf(item);
        public void Insert(int index, PlayerEffect item) => _effects.Insert(index, item);
        public bool Remove(PlayerEffect item) => _effects.Remove(item);
        public void RemoveAt(int index) => _effects.RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => _effects.GetEnumerator();
        public void RemoveAll(Predicate<PlayerEffect> value) => _effects.RemoveAll(value);
        public void AddRange(List<PlayerEffect> powerUpEffects) => _effects.AddRange(powerUpEffects);
    }
}
