namespace GraphUpdateDataAccess.Context
{
    public class GraphUpdateContextSetup : GraphUpdateContextBase
    {
        public GraphUpdateContextSetup() 
        {
            //// Uncomment to recreate each time.  
            //// Not needed as the unit tests delete any inserted data.
            //// Easier to just delete the database.
            // Database.SetInitializer<GraphUpdateContextSetup>(new DropCreateDatabaseAlways<GraphUpdateContextSetup>());
        }
    }
}
