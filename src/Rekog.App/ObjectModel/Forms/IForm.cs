using System.Collections.Generic;
using Rekog.App.Model;

namespace Rekog.App.ObjectModel.Forms
{
    public interface IForm<TModel>
        where TModel : ModelBase
    {
        IReadOnlyCollection<TModel> Models { get; }

        void Set(TModel model);

        void Set(IEnumerable<TModel> models);

        void Update();
    }
}
