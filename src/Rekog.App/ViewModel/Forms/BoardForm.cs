using Rekog.App.Forms;
using Rekog.App.Model;
using Rekog.App.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class BoardForm : ModelForm<BoardModel>
    {
        public BoardForm(UndoContext undoContext) : base(undoContext)
        {
        }

        public ModelFormProperty Background => GetProperty(nameof(BoardModel.Background));
    }
}
