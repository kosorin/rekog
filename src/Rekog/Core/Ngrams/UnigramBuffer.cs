namespace Rekog.Core.Ngrams
{
    public class UnigramBuffer : INgramBuffer
    {
        public void Skip()
        {
            // Nothing to do
        }

        public bool Next(char character, out string ngramValue)
        {
            ngramValue = character.ToString();
            return true;
        }
    }
}
