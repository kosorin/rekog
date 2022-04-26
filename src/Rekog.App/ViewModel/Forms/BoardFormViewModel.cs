using System.Collections.Generic;
using Rekog.App.Model;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.ViewModel.Forms
{
    public class BoardFormViewModel : FormViewModel<BoardModel>
    {
        public BoardFormViewModel()
        {
            Properties = new FormProperty<BoardModel>[]
            {
                Background = FormProperty.Reference(this, x => x.Background),
            };
        }

        public override IReadOnlyCollection<FormProperty<BoardModel>> Properties { get; }

        public FormProperty<BoardModel, string?> Background { get; }
    }
}
