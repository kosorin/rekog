using Rekog.App.Model;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.ViewModel.Forms
{
    public class BoardFormViewModel : FormViewModel
    {
        public BoardFormViewModel(params BoardModel[] models) : base(models)
        {
            Name = FormProperty.Reference(models, x => x.Name);
            Author = FormProperty.Reference(models, x => x.Author);
            Notes = FormProperty.Reference(models, x => x.Notes);
            Background = FormProperty.Reference(models, x => x.Background);
        }

        public IFormProperty<string?> Name { get; }

        public IFormProperty<string?> Author { get; }

        public IFormProperty<string?> Notes { get; }

        public IFormProperty<string?> Background { get; }
    }
}
