using System.Collections.Generic;
using Rekog.App.Model;

namespace Rekog.App.ObjectModel.Forms
{
    public interface IForm<TModel> : IObservableObject
        where TModel : ModelBase
    {
        IReadOnlyCollection<TModel> Models { get; }

        public bool IsSet { get; }

        void Set(TModel model);

        void Set(IEnumerable<TModel> models);

        void Clear();
    }
}
