using System;
using System.Threading.Tasks;

namespace Rekog.Commands
{
    public abstract class CommandBase
    {
        public abstract Task Handle();
    }
}
