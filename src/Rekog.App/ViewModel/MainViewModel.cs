using Rekog.App.Model;
using Rekog.App.Model.Kle;
using Rekog.App.ObjectModel;
using System.Collections.Generic;
using System.Linq;

namespace Rekog.App.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public MainViewModel()
        {
            ParseKleRawDataCommand = new DelegateCommand<string>(ParseKleRawData);
        }

        private BoardViewModel? _board;
        public BoardViewModel? Board
        {
            get => _board;
            set => Set(ref _board, value);
        }

        public DelegateCommand<string> ParseKleRawDataCommand { get; }

        private void ParseKleRawData(string? kleRawData)
        {
            var kleKeys = Parse(kleRawData);
            if (kleKeys == null)
            {
                Board = null;
            }
            else
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
                    return null;
                }
                else
                {
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
}
