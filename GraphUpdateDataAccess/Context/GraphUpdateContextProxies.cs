using System.Data.Entity;

namespace GraphUpdateDataAccess.Context
{
    public class GraphUpdateContextProxies : GraphUpdateContextBase
    {
        public GraphUpdateContextProxies()
        {
            Database.SetInitializer<GraphUpdateContextNoProxies>(null);
            this.Configuration.ProxyCreationEnabled = true;
        }
    }
}
