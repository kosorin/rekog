namespace Rekog.Core.Layouts
{
    public class LayoutNgram
    {
        public LayoutNgram(string value, Key[] keys)
        {
            Value = value;
            Keys = keys;
        }

        public string Value { get; set; }

        public Key[] Keys { get; set; }

        public double Effort { get; set; }
    }
}
