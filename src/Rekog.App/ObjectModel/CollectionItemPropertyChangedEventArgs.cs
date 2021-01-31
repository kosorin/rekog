namespace Rekog.App.ObjectModel
{
    public class CollectionItemPropertyChangedEventArgs
    {
        public CollectionItemPropertyChangedEventArgs(string? propertyName)
        {
            PropertyName = propertyName;
        }

        public string? PropertyName { get; }
    }
}
