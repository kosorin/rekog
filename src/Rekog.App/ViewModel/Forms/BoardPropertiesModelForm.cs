using Rekog.App.Model;
using Rekog.App.ObjectModel;
using Rekog.App.ObjectModel.Forms;
using Rekog.App.ObjectModel.Undo;

namespace Rekog.App.ViewModel.Forms
{
    public class BoardPropertiesModelForm : ModelForm<BoardModel>
    {
        public BoardPropertiesModelForm(UndoContext undoContext) : base(undoContext)
        {
        }

        public ModelFormProperty Name => GetProperty(nameof(BoardModel.Name));

        public ModelFormProperty Author => GetProperty(nameof(BoardModel.Author));

        public ModelFormProperty Notes => GetProperty(nameof(BoardModel.Notes));
    }
}
