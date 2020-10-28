using System;
using System.Threading.Tasks;

namespace Rekog.Commands
{
    public class SubRootCommand : CommandBase
    {
        public override Task Handle()
        {
            throw new NotSupportedException();
        }
    }
}
