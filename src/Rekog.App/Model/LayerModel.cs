namespace Rekog.App.Model
{
    public class LayerModel : ModelBase
    {
        private string _name = string.Empty;

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
    }
}
