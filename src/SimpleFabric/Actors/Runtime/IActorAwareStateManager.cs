using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleFabric.Actors.Runtime
{
    public interface IActorAwareStateManager : IActorStateManager
    {
        Actor Actor { get; set; }
        void Initialize();
    }
}
