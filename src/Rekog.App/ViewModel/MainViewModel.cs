using System;
using Rekog.App.Model;
using Rekog.App.Model.Kle;
using Rekog.App.ObjectModel;

namespace Rekog.App.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private BoardViewModel _board = new BoardViewModel(new BoardModel());

        public MainViewModel()
        {
            ParseKleRawDataCommand = new DelegateCommand<string>(ParseKleRawData);
            ParseKleJsonCommand = new DelegateCommand<string>(ParseKleJson);
        }

        public BoardViewModel Board
        {
            get => _board;
            set => Set(ref _board, value);
        }

        public DelegateCommand<string> ParseKleRawDataCommand { get; }

        public DelegateCommand<string> ParseKleJsonCommand { get; }

        private void ParseKleRawData(string? kleRawData)
        {
            TrySetBoard(kleRawData, KleParser.ParseRawData);
        }

        private void ParseKleJson(string? kleJson)
        {
            TrySetBoard(kleJson, KleParser.ParseJson);
        }

        private void TrySetBoard(string? kleInput, Func<string, KleBoard> parser)
        {
            var kleBoard = ParseKle(kleInput, parser);
            if (kleBoard != null)
            {
                Board = new BoardViewModel(BoardModel.FromKle(kleBoard));
            }
        }

        private KleBoard? ParseKle(string? kleInput, Func<string, KleBoard> parser)
        {
            if (string.IsNullOrWhiteSpace(kleInput))
            {
                return new KleBoard();
            }

            try
            {
                return parser.Invoke(kleInput);
            }
            catch
            {
                return null;
            }
        }
    }
}
