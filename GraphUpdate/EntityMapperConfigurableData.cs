using System;
using System.Collections.Generic;

namespace GraphUpdate
{
    /// <summary>
    /// Used by EntityMapperConfigurableFactory.  Allows developer to specify column and
    /// navigation properties.  Used with EntityMapperConfigurableFactory.
    /// </summary>
    public class EntityMapperConfigurableData
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public EntityMapperConfigurableData()
        {
            this.MappingPairs = new List<PropertyPair>();
            this.NavigationPairs = new List<PropertyPair>();
        }

        /// <summary>
        /// The model entity type.
        /// </summary>
        public Type ModelType { get; set; }

        /// <summary>
        /// The db entity type.
        /// </summary>
        public Type DbType { get; set; }

        /// <summary>
        /// Holds the primary key propert names.
        /// </summary>
        public PropertyPair IdProperties { get; set; }

        /// <summary>
        /// Holds the simple property mappings.  Each item in the list
        /// is a property to map.  The property names can be different.
        /// </summary>
        public IList<PropertyPair> MappingPairs { get; set; }

        /// <summary>
        /// Holds the navigation properties.  Each item in the list is a
        /// navigation property to map.  The property names can be different.
        /// </summary>
        public IList<PropertyPair> NavigationPairs { get; set; }
    }
}
