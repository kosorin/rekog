namespace Rekog.Core.Layouts
{
    public class LayoutNgram
    {
        public LayoutNgram(string value, Key[] keys)
        {
            Value = value;
            Keys = keys;
        }

        public string Value { get; }

        public Key[] Keys { get; }

        public double Effort { get; set; }
    }
}
