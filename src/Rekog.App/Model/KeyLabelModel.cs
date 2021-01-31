namespace Rekog.App.Model
{
    public class KeyLabelModel : ModelBase
    {
        public int Position { get; set; }

        public int Size { get; set; }

        public string Color { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;
    }
}
