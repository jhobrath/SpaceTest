using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core2.Services
{
    public interface IClearable
    {
        void Clear();
    }

    public interface IClearableServiceClearer
    {
        void Clear();
    }

    public class ClearableServiceClearer : IClearableServiceClearer
    {
        private readonly IEnumerable<IClearable> _clearableServices = [];

        public ClearableServiceClearer(IEnumerable<IClearable> clearableServices)
        {
            _clearableServices = clearableServices;
        }

        public void Clear()
        {
            foreach (var service in _clearableServices)
                service.Clear();
        }
    }
}
