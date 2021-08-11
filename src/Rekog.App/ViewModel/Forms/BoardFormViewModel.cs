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
                Name = FormProperty<BoardModel>.Reference(this, x => x.Name),
                Author = FormProperty<BoardModel>.Reference(this, x => x.Author),
                Notes = FormProperty<BoardModel>.Reference(this, x => x.Notes),
                Background = FormProperty<BoardModel>.Reference(this, x => x.Background),
            };
        }

        public override IReadOnlyCollection<FormProperty<BoardModel>> Properties { get; }

        public FormProperty<BoardModel, string?> Name { get; }

        public FormProperty<BoardModel, string?> Author { get; }

        public FormProperty<BoardModel, string?> Notes { get; }

        public FormProperty<BoardModel, string?> Background { get; }
    }
}
