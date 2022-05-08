using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.ObjectModel.Forms;
using Rekog.App.ObjectModel.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class BoardFileModelForm : ModelForm<BoardModel>
    {
        public BoardFileModelForm(UndoContext undoContext) : base(undoContext)
        {
        }
    }
}
