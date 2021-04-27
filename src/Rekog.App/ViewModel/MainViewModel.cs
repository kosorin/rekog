using System.Collections.Generic;
using System.Linq;
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
        }

        public BoardViewModel Board
        {
            get => _board;
            set => Set(ref _board, value);
        }

        public DelegateCommand<string> ParseKleRawDataCommand { get; }

        private void ParseKleRawData(string? kleRawData)
        {
            var kleKeys = Parse(kleRawData);
            if (kleKeys != null)
            {
                var boardModel = new BoardModel
                {
                    Keys = new ObservableObjectCollection<KeyModel>(kleKeys.Select(KeyModel.FromKle)),
                };
                Board = new BoardViewModel(boardModel);
            }

            static List<KleKey>? Parse(string? kleRawData)
            {
                if (string.IsNullOrWhiteSpace(kleRawData))
                {
                    return new List<KleKey>();
                }

                try
                {
                    return KleParser.ParseRawData(kleRawData);
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
