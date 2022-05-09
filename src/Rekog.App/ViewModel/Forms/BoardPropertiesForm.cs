using Rekog.App.Forms;
using Rekog.App.Model;
using Rekog.App.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class BoardPropertiesForm : ModelForm<BoardModel>
    {
        public BoardPropertiesForm(UndoContext undoContext) : base(undoContext)
        {
        }

        public ModelFormProperty Name => GetProperty(nameof(BoardModel.Name));

        public ModelFormProperty Author => GetProperty(nameof(BoardModel.Author));

        public ModelFormProperty Notes => GetProperty(nameof(BoardModel.Notes));
    }
}
