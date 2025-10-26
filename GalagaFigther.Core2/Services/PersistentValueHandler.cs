using GalagaFighter.Core2.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IPersistentValueHandler : IClearable
    {
        void Register(Action whenTrue, Action whenFalse, Func<bool> condition);
        void Update(float frameTime);
    }
    public class PersistentValueHandler : IPersistentValueHandler
    {
        private readonly List<PersistentCondition> _conditions = [];

        public void Clear()
        {
            _conditions.Clear();
        }

        public void Register(Action whenTrue, Action whenFalse, Func<bool> condition)
        {
            _conditions.Add(new PersistentCondition
            {
                WhenFalse = whenFalse,
                WhenTrue = whenTrue,
                Condition = condition
            });
        }

        public void Update(float frameTime)
        {
            foreach(var condition in _conditions)
            {
                if (condition.Condition())
                    condition.WhenTrue();
                else
                    condition.WhenFalse();
            }
        }

        private class PersistentCondition
        {
            public Action WhenTrue { get; set; }
            public Action WhenFalse { get; set; }
            public Func<bool> Condition { get; set; }
        }
    }
}
