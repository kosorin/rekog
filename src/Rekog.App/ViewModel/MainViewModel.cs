using System;
using Rekog.App.Model;
using Rekog.App.Model.Kle;
using Rekog.App.ObjectModel;

namespace Rekog.App.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        private BoardViewModel _board;

        public MainViewModel()
        {
            _board = GetNewKeyboard();

            NewKeyboardCommand = new DelegateCommand(NewKeyboard);
            ParseKleRawDataCommand = new DelegateCommand<string>(ParseKleRawData);
            ParseKleJsonCommand = new DelegateCommand<string>(ParseKleJson);
        }

        public BoardViewModel Board
        {
            get => _board;
            set => Set(ref _board, value);
        }

        public DelegateCommand NewKeyboardCommand { get; }

        public DelegateCommand<string> ParseKleRawDataCommand { get; }

        public DelegateCommand<string> ParseKleJsonCommand { get; }

        private void NewKeyboard()
        {
            Board = GetNewKeyboard();
        }

        private BoardViewModel GetNewKeyboard()
        {
            var kle = KleParser.ParseRawData("[\"~\\n`\",\"!\\n1\",\"@\\n2\",\"#\\n3\",\"$\\n4\",\"%\\n5\",\"^\\n6\",\"&\\n7\",\"*\\n8\",\"(\\n9\",\")\\n0\",\"_\\n-\",\"+\\n=\",{w:2},\"Backspace\"],[{w:1.5},\"Tab\",\"Q\",\"W\",\"E\",\"R\",\"T\",\"Y\",\"U\",\"I\",\"O\",\"P\",\"{\\n[\",\"}\\n]\",{w:1.5},\"|\\n\\\\\"],[{w:1.75},\"Caps Lock\",\"A\",\"S\",\"D\",\"F\",\"G\",\"H\",\"J\",\"K\",\"L\",\":\\n;\",\"\\\"\\n'\",{w:2.25},\"Enter\"],[{w:2.25},\"Shift\",\"Z\",\"X\",\"C\",\"V\",\"B\",\"N\",\"M\",\"<\\n,\",\">\\n.\",\"?\\n/\",{w:2.75},\"Shift\"],[{w:1.25},\"Ctrl\",{w:1.25},\"Win\",{w:1.25},\"Alt\",{a:7,w:6.25},\"\",{a:4,w:1.25},\"Alt\",{w:1.25},\"Win\",{w:1.25},\"Menu\",{w:1.25},\"Ctrl\"]");
            var model = BoardModel.FromKle(kle);
            model = new BoardModel();
            return new BoardViewModel(model);
        }

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
