namespace Rekog.Core.Layouts.Analyzers
{
    internal class SameHandLongRollAnalyzer : TrigramAnalyzer<(Hand hand, Roll roll)>
    {
        public SameHandLongRollAnalyzer() : base("Same hand long roll")
        {
        }

        protected override bool TryGetValue(Key firstKey, Key secondKey, Key thirdKey, out ((Hand, Roll), double?) value)
        {
            if (firstKey.GetHandRoll(secondKey) is not Roll.None and Roll handRoll && handRoll == secondKey.GetHandRoll(thirdKey))
            {
                var effort = handRoll == Roll.Outward ? 0.3 : (double?)null;

                value = ((firstKey.Hand, handRoll), effort);
                return true;
            }
            value = default;
            return false;
        }
    }
}
