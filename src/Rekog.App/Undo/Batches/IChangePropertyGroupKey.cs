using System.Reflection;

namespace Rekog.App.Undo.Batches
{
    public interface IChangePropertyGroupKey
    {
        bool CanCoalesce(IChangePropertyGroupKey otherGroupKey);

        bool AllowPropertyInfo(PropertyInfo propertyInfo);
    }
}