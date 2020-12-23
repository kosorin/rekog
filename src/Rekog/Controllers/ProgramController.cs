using System;
using System.Threading;

namespace Rekog.Controllers
{
    public class ProgramController
    {
        private readonly Func<CorpusController> _corpusControllerFactory;
        private readonly Func<LayoutController> _layoutControllerFactory;

        public ProgramController(Func<CorpusController> corpusControllerFactory, Func<LayoutController> layoutControllerFactory)
        {
            _corpusControllerFactory = corpusControllerFactory;
            _layoutControllerFactory = layoutControllerFactory;
        }

        public void Run(CancellationToken cancellationToken)
        {
            var corpusData = _corpusControllerFactory.Invoke().GetCorpusData(cancellationToken);
            if (corpusData == null)
            {
                return;
            }

            _layoutControllerFactory.Invoke().Analyze(corpusData);
        }
    }
}
