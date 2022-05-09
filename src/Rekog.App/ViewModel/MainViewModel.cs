using System.Linq;
using Rekog.App.Kle;
using Rekog.App.Model;
using Rekog.App.ObjectModel;

namespace Rekog.App.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private BoardViewModel _board;

        public MainViewModel()
        {
            _board = GetNewBoard();

            NewBoardCommand = new DelegateCommand(NewBoard);
            ParseKleRawDataCommand = new DelegateCommand<string>(ParseKleRawData);
            ParseKleJsonCommand = new DelegateCommand<string>(ParseKleJson);
        }

        public BoardViewModel Board
        {
            get => _board;
            set => Set(ref _board, value);
        }

        public DelegateCommand NewBoardCommand { get; }

        public DelegateCommand<string> ParseKleRawDataCommand { get; }

        public DelegateCommand<string> ParseKleJsonCommand { get; }

        private void NewBoard()
        {
            Board = GetNewBoard();
        }

        private BoardViewModel GetNewBoard()
        {
            return new BoardViewModel(new BoardModel
            {
                Layers = new ObservableDictionary<LayerId, LayerModel>(new[]
                {
                    new LayerModel(0) { Name = "Base", },
                    new LayerModel(1) { Name = "Lower", },
                    new LayerModel(2) { Name = "Raise", },
                    new LayerModel(3) { Name = "Adjust", },
                }.ToDictionary(x => x.Id, x => x)),
            });
        }

        private void ParseKleRawData(string? kleRawData)
        {
            if (!string.IsNullOrWhiteSpace(kleRawData) && KleConverter.ConvertRawData(kleRawData) is { } boardModel)
            {
                Board = new BoardViewModel(boardModel);
            }
            else
            {
                NewBoard();
            }
        }

        private void ParseKleJson(string? kleJson)
        {
            if (!string.IsNullOrWhiteSpace(kleJson) && KleConverter.ConvertJson(kleJson) is { } boardModel)
            {
                Board = new BoardViewModel(boardModel);
            }
            else
            {
                NewBoard();
            }
        }
    }
}
