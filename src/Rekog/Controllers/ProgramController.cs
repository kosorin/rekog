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
            var corpusAnalysisData = _corpusControllerFactory.Invoke().GetCorpusAnalysisData(cancellationToken);
            if (corpusAnalysisData == null)
            {
                return;
            }

            _layoutControllerFactory.Invoke().Analyze(corpusAnalysisData);
        }
    }
}
