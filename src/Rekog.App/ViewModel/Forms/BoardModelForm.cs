using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.ObjectModel.Forms;
using Rekog.App.ObjectModel.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class BoardModelForm : ModelForm<BoardModel>
    {
        public BoardModelForm(UndoContext undoContext) : base(undoContext)
        {
        }

        public ModelFormProperty Background => GetProperty(nameof(BoardModel.Background));
    }
}
