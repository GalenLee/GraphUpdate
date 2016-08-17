# Entity Framework Graph Update (Graph Diff)

- Update a disconnected detached graph.
- Automatically handles insert, updates and delete.
- Extensible by allowing for custom mappers.

This solution demonstrates three ways to implement an Entity Framework Graph Update.
- EntityMapperAutomaticFactory (uses reflection and automatically determines mapping)
- EntityMapperConfigurableFactory (uses reflection and user can configure mappings)
- EntityMapperCustomFactory (user needs to supply mapping code)
- EntityMapperCachedFactor (wrapper around any mapper - EntityMapperAutomaticFactory)

The magic happens in the GraphUpdater class which has a surprisingly small amount of code.

Look at the unit tests to see how to use.

TODO:
- Unit test exceptions
- Create AutoMapper EntityMapper




