using GalagaFighter.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalagaFighter.Core.CPU
{
    public class CpuKeyMappings : IInputMappings
    {
        private readonly ICpuDecisionMaker _cpuDecisionMaker;

        public CpuKeyMappings(ICpuDecisionMaker decisionMaker)
        {
            _cpuDecisionMaker = decisionMaker;
        }

        public bool IsMoveLeftDown() => _cpuDecisionMaker.IsMoveLeftDown();
        public bool IsMoveRightDown() => _cpuDecisionMaker.IsMoveRightDown();
        public bool IsShootDown()     => _cpuDecisionMaker.IsShootDown();
        public bool IsSwitchDown()    => _cpuDecisionMaker.IsSwitchDown();
    }
}
