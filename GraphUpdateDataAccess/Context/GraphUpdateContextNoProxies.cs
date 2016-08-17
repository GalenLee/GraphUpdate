using System.Data.Entity;

namespace GraphUpdateDataAccess.Context
{
    public class GraphUpdateContextNoProxies : GraphUpdateContextBase
    {
        public GraphUpdateContextNoProxies()
        {
            Database.SetInitializer<GraphUpdateContextNoProxies>(null);
            this.Configuration.ProxyCreationEnabled = false;
        }
    }
}

