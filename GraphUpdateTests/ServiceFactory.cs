using GraphUpdate;
using GraphUpdateDataAccess.Context;
using GraphUpdateDataAccess.Service;

namespace GraphUpdateTests
{
    public abstract class ServiceFactory
    {
        protected IEntityMapperFactory MapperFactory;
        protected ServiceFactory(IEntityMapperFactory mapperFactory)
        {
            this.MapperFactory = mapperFactory;
        }

        public abstract Service Create();

    }

    public class ContextNoProxiesServiceFactory: ServiceFactory
    {
        public ContextNoProxiesServiceFactory(IEntityMapperFactory mapperFactory)
            : base(mapperFactory)
        {
        }

        public override Service Create()
        {
            var context = new GraphUpdateContextNoProxies();
            var a = context.Configuration.ProxyCreationEnabled;
            var updater = new GraphUpdater(this.MapperFactory);
            return new Service(context, updater);
        }
    }


    public class ContextProxiesServiceFactory : ServiceFactory
    {
        public ContextProxiesServiceFactory(IEntityMapperFactory mapperFactory)
            : base(mapperFactory)
        {
        }

        public override Service Create()
        {
            var context = new GraphUpdateContextProxies();
            var updater = new GraphUpdater(this.MapperFactory);
            return new Service(context, updater);
        }
    }
}