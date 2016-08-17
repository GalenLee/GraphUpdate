namespace GraphUpdate
{
    /// <summary>
    /// Holds a pair of property names.
    /// </summary>
    public class PropertyPair
    {
        /// <summary>
        /// The name of the model entity property.
        /// </summary>
        public string ModelProperty { get; set; }

        /// <summary>
        /// The name of the db entity property.
        /// </summary>
        public string DbProperty { get; set; }
    }
}
