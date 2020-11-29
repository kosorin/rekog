namespace Rekog.Core.Corpora
{
    public class CorpusReader
    {
        public CorpusReader(CorpusFile file)
        {
            File = file;
        }

        public CorpusFile File { get; }
    }
}
