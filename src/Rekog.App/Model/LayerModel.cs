namespace Rekog.App.Model
{
    public record LayerId(int Value)
    {
        public static implicit operator LayerId(int value)
        {
            return new LayerId(value);
        }
    }

    public class LayerModel : ModelBase
    {
        private string _name = string.Empty;

        public LayerModel(LayerId id)
        {
            Id = id;
        }

        public LayerId Id { get; }

        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }
    }
}
