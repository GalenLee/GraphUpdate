using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphUpdate
{
    /// <summary>
    /// Interface for entity mapper factories.
    /// </summary>
    public interface IEntityMapperFactory
    {
        /// <summary>
        /// Creates or gets the entity mapper for the given db entity type.
        /// Parameter modelEntityType can be null.  In this case only the 
        /// db entity mapping navigation would be used for removing entities.
        /// </summary>
        IEntityMapper Create(Type modelEntityType, Type dbEntityType);
    }
}
