
namespace AssetSquirrel.WebApp.Mapper
{
    public interface IGenericMapper<T, U>
    {
        IEnumerable<U> Map(IEnumerable<T> request);
        IEnumerable<T> Map(IEnumerable<U> request);
        U Map(T request);
        T Map(U request);
    }
}