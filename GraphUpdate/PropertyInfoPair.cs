using System.Reflection;

namespace GraphUpdate
{
    /// <summary>
    /// Holds a pair of PropertyInfos.  
    /// </summary>
    public class PropertyInfoPair
    {
        /// <summary>
        /// PropertyInfo for a model entity property.
        /// </summary>
        public PropertyInfo ModelPropertyInfo { get; set; }

        /// <summary>
        /// PropertyInfo for a db entity property.
        /// </summary>
        public PropertyInfo DbPropertyInfo { get; set; }
    }
}
