using Rekog.App.Forms;
using Rekog.App.Model;
using Rekog.App.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class BoardFileForm : ModelForm<BoardModel>
    {
        public BoardFileForm(UndoContext undoContext) : base(undoContext)
        {
        }
    }
}
