using AutoMapper;

namespace AssetSquirrel.UseCases.Mapper
{
    public class GenericMapper<T, U> : IGenericMapper<T, U>
    {
        private IMapper mapper;

        public GenericMapper()
        {
            try
            {
                mapper = new MapperConfiguration(config =>
                {
                    config.CreateMap<U, T>().ReverseMap();
                }).CreateMapper();
            }
            catch (Exception e)
            {
                //ErrorHandling.InsertError("iKomfort.ApplicationGlobal.AbstractClasses", "GenericMapper", "GenericMapper", e);
            }
        }

        public IEnumerable<T> Map(IEnumerable<U> request) => mapper?.Map<IEnumerable<T>>(request);

        public IEnumerable<U> Map(IEnumerable<T> request) => mapper.Map<IEnumerable<U>>(request);

        public T Map(U request) => mapper.Map<T>(request);

        public U Map(T request) => mapper.Map<U>(request);
    }
}
