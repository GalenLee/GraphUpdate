using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GraphUpdateDataAccess.DbModel;
using GraphUpdate;
using GraphUpdateDataAccess.CustomMappers;

namespace GraphUpdateTests
{
    using GraphUpdateDataAccess.DomainModel;

    public class Helper
    {
        public static string Serialize(ParentA parentA)
        {
            var sb = new StringBuilder();
            sb.Append("ParentA:" + parentA.P1 + "," + parentA.P2 + "," + parentA.P3);
            foreach (var childA1 in parentA.ChildA1s.OrderBy(x => x.Id))
            {
                sb.Append(";ChildA1:" + childA1.P1 + "," + childA1.P2 + "," + childA1.P3);
                foreach (var grandChildA1 in childA1.GrandChildA1s.OrderBy(x => x.Id))
                {
                    sb.Append(";GrandChildA1:" + grandChildA1.P1 + "," + grandChildA1.P2 + "," + grandChildA1.P3);
                }
            }

            return sb.ToString();
        }

        public static string Serialize(DmParentB dmParentB)
        {
            var sb = new StringBuilder();
            sb.Append("DmParentB:" + dmParentB.Field1 + "," + dmParentB.Field2 + "," + dmParentB.Field3);
            foreach (var dmChildB in dmParentB.ChildBs)
            {
                sb.Append(";DmChildB:" + dmChildB.Field1);
                foreach (var dmGrandChildB1 in dmChildB.GrandChildB1s)
                {
                    sb.Append(";DmGrandChildB1:" + dmGrandChildB1.Field1 + "," + dmGrandChildB1.Field2);
                }
                foreach (var dmGrandChildB2 in dmChildB.GrandChildB2s)
                {
                    sb.Append(";DmGrandChildB2:" + dmGrandChildB2.Field1);
                }
            }
            return sb.ToString();
        }

        public static IEntityMapperFactory CreateAutomaticFactory()
        {
            return new EntityMapperAutomaticFactory("Id");
        }

        public static IEntityMapperFactory CreateCustomFactory()
        {
            var maps = new Dictionary<Type, IEntityMapper>();
            maps.Add(typeof(ParentA), new ParentAMapper());
            maps.Add(typeof(ChildA1), new ChildA1Mapper());
            maps.Add(typeof(GrandChildA1), new GrandChildA1Mapper());
            return new EntityMapperCustomFactory(maps);
        }

        public static IEntityMapperFactory CreateConfigurableFactory()
        {
            var maps = new List<EntityMapperConfigurableData>();
            maps.Add(new EntityMapperConfigurableData
            {
                ModelType = typeof(ParentA),
                DbType = typeof(ParentA),
                IdProperties = new PropertyPair { ModelProperty = "Id", DbProperty = "Id" },
                MappingPairs = new List<PropertyPair>
                {
                    new PropertyPair { ModelProperty = "P1", DbProperty = "P1" },
                    new PropertyPair { ModelProperty = "P2", DbProperty = "P2" },
                    new PropertyPair { ModelProperty = "P3", DbProperty = "P3" }
                },
                NavigationPairs = new List<PropertyPair>
                {
                    new PropertyPair { ModelProperty = "ChildA1s", DbProperty = "ChildA1s" },
                    new PropertyPair { ModelProperty = "ChildA2s", DbProperty = "ChildA2s" },
                },
            });
            maps.Add(new EntityMapperConfigurableData
            {
                ModelType = typeof(ChildA1),
                DbType = typeof(ChildA1),
                IdProperties = new PropertyPair { ModelProperty = "Id", DbProperty = "Id" },
                MappingPairs = new List<PropertyPair>
                {
                    new PropertyPair { ModelProperty = "P1", DbProperty = "P1" },
                    new PropertyPair { ModelProperty = "P2", DbProperty = "P2" },
                    new PropertyPair { ModelProperty = "P3", DbProperty = "P3" }
                },
                NavigationPairs = new List<PropertyPair>
                {
                    new PropertyPair { ModelProperty = "GrandChildA1s", DbProperty = "GrandChildA1s" },
                },
            });
            maps.Add(new EntityMapperConfigurableData
            {
                ModelType = typeof(ChildA2),
                DbType = typeof(ChildA2),
                IdProperties = new PropertyPair { ModelProperty = "Id", DbProperty = "Id" },
                MappingPairs = new List<PropertyPair>
                {
                    new PropertyPair { ModelProperty = "P1", DbProperty = "P1" },
                    new PropertyPair { ModelProperty = "P2", DbProperty = "P2" },
                    new PropertyPair { ModelProperty = "P3", DbProperty = "P3" }
                },
            });
            maps.Add(new EntityMapperConfigurableData
            {
                ModelType = typeof(GrandChildA1),
                DbType = typeof(GrandChildA1),
                IdProperties = new PropertyPair { ModelProperty = "Id", DbProperty = "Id" },
                MappingPairs = new List<PropertyPair>
                {
                    new PropertyPair { ModelProperty = "P1", DbProperty = "P1" },
                    new PropertyPair { ModelProperty = "P2", DbProperty = "P2" },
                    new PropertyPair { ModelProperty = "P3", DbProperty = "P3" }
                }
            });
            return new EntityMapperConfigurableFactory(maps);
        }
    }
}
