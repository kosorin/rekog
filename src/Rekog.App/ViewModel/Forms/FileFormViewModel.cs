using System.Collections.Generic;
using Rekog.App.Model;
using Rekog.App.ObjectModel.Forms;

namespace Rekog.App.ViewModel.Forms
{
    public class FileFormViewModel : FormViewModel<BoardModel>
    {
        public FileFormViewModel()
        {
            Properties = new FormProperty<BoardModel>[]
            {
            };
        }

        public override IReadOnlyCollection<FormProperty<BoardModel>> Properties { get; }
    }
}
