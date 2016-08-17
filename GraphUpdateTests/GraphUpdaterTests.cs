using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;
using GraphUpdateDataAccess.DbModel;
using System.Diagnostics;
using GraphUpdate;
using GraphUpdateDataAccess.DomainModel;

namespace GraphUpdateTests
{
    [TestClass]
    public class GraphUpdaterTests
    {
        [TestMethod]
        public void InsertUpdateDeleteTestsA()
        {
            this.RunInsertUpdateDeleteTestsA(new ContextNoProxiesServiceFactory(Helper.CreateAutomaticFactory()));
            this.RunInsertUpdateDeleteTestsA(new ContextNoProxiesServiceFactory(Helper.CreateCustomFactory()));
            this.RunInsertUpdateDeleteTestsA(new ContextNoProxiesServiceFactory(Helper.CreateConfigurableFactory()));

            // FYI - The other factories cache the entity mappers.
            var cachedFactory = new EntityMapperCachedFactory(Helper.CreateAutomaticFactory());
            this.RunInsertUpdateDeleteTestsA(new ContextNoProxiesServiceFactory(cachedFactory));
            Assert.AreEqual(8, cachedFactory.TotalCreateCalls);

            this.RunInsertUpdateDeleteTestsA(new ContextProxiesServiceFactory(Helper.CreateAutomaticFactory()));
            this.RunInsertUpdateDeleteTestsA(new ContextProxiesServiceFactory(Helper.CreateCustomFactory()));
            this.RunInsertUpdateDeleteTestsA(new ContextProxiesServiceFactory(Helper.CreateConfigurableFactory()));
        }

        public void RunInsertUpdateDeleteTestsA(ServiceFactory serviceFactory)
        {
            // Create model graph and insert.
            var service = serviceFactory.Create();
            var modelParent1 = new ParentA
            {
                // Ids <= 0 are new records.
                P1 = "P1", P2 = 10, P3 = 100, Id = 0,  
                ChildA1s = new HashSet<ChildA1>
                {
                    new ChildA1() {P1 = "ABC", P2 = new DateTime(2016,1,1), P3 = null, Id = 0 },
                    new ChildA1()
                        {
                            P1 = "XYZ", P2 = new DateTime(2017,12,31), P3 = new DateTime(2018,1,1), Id = 0,
                            GrandChildA1s = new HashSet<GrandChildA1>
                            {
                                new GrandChildA1 { P1 = 1001, P2 = 8, P3 = true },
                                new GrandChildA1 { P1 = 1002, P2 = 9, P3 = null },
                            }
                        }
                }
            };
            var dbParent1 = service.InsertParentA(modelParent1);

            // Get inserted graph and test
            service = serviceFactory.Create();
            var modelParent2 = service.GetParentANoTracking(dbParent1.Id);
            Assert.AreEqual("ParentA:P1,10,100;" + 
                "ChildA1:ABC,1/1/2016 12:00:00 AM,;" + 
                "ChildA1:XYZ,12/31/2017 12:00:00 AM,1/1/2018 12:00:00 AM;" +
                "GrandChildA1:1001,8,True;" + 
                "GrandChildA1:1002,9,", Helper.Serialize(modelParent2));

            // Simple graph update
            modelParent2.P3 = null;
            modelParent2.ChildA1s.ToList()[0].P1 = "abc";
            modelParent2.ChildA1s.ToList()[1].GrandChildA1s.ToList()[1].P3 = false;
            var dbParent2 = service.GetParentA(modelParent2.Id);
            service.UpdateParentA(modelParent2, dbParent2);

            // Get updated graph and test
            service = serviceFactory.Create();
            var modelParent3 = service.GetParentANoTracking(dbParent2.Id);
            Assert.AreEqual("ParentA:P1,10,;" +
                "ChildA1:abc,1/1/2016 12:00:00 AM,;" +
                "ChildA1:XYZ,12/31/2017 12:00:00 AM,1/1/2018 12:00:00 AM;" +
                "GrandChildA1:1001,8,True;" +
                "GrandChildA1:1002,9,False", Helper.Serialize(modelParent3));

            // Complex graph update
            modelParent3.P3 = 200;
            modelParent3.ChildA1s.Add(new ChildA1()
            {
                P1 = "QWERTY",
                P2 = new DateTime(2010, 6, 1),
                P3 = new DateTime(2010, 6, 1, 5, 30, 30),
                GrandChildA1s = new HashSet<GrandChildA1>
                            {
                                new GrandChildA1 { P1 = 1003, P2 = 2, P3 = null },
                                new GrandChildA1 { P1 = 1004, P2 = 4, P3 = false },
                            }
            });
            var child1 = modelParent3.ChildA1s.ToList()[0];
            var child2 = modelParent3.ChildA1s.ToList()[1];
            modelParent3.ChildA1s.Remove(child1);
            child2.P2 = new DateTime(2000,1,1);
            child2.GrandChildA1s.Remove(child2.GrandChildA1s.First());
            child2.GrandChildA1s.Add(new GrandChildA1 { P1 = 1005, P2 = 16, P3 = true });
            var dbParent3 = service.GetParentA(modelParent3.Id);
            service.UpdateParentA(modelParent3, dbParent3);

            // Get updated graph and test
            service = serviceFactory.Create();
            var modelParent4 = service.GetParentANoTracking(dbParent3.Id);
            Assert.AreEqual("ParentA:P1,10,200;" +
                "ChildA1:XYZ,1/1/2000 12:00:00 AM,1/1/2018 12:00:00 AM;" +
                "GrandChildA1:1002,9,False;" +
                "GrandChildA1:1005,16,True;" +
                "ChildA1:QWERTY,6/1/2010 12:00:00 AM,6/1/2010 5:30:30 AM;" +
                "GrandChildA1:1003,2,;" +
                "GrandChildA1:1004,4,False", Helper.Serialize(modelParent4));

            // Remove graph and test
            service = serviceFactory.Create();
            var dbParent4 = service.GetParentA(modelParent4.Id);
            service.DeleteParentA(dbParent4);
            Assert.IsNull(service.GetParentA(modelParent4.Id));
        }

        [TestMethod]
        public void InsertUpdateDeleteTestsB()
        {
            this.RunInsertUpdateDeleteTestsB(new ContextNoProxiesServiceFactory(Helper.CreateAutomaticFactory()));
            this.RunInsertUpdateDeleteTestsB(new ContextProxiesServiceFactory(Helper.CreateAutomaticFactory()));
        }

        public void RunInsertUpdateDeleteTestsB(ServiceFactory serviceFactory)
        {
            // Create graph and insert.
            var service = serviceFactory.Create();
            var modelParent1 = new DmParentB
            {
                Id = -1,
                Field1 = "F1",
                Field2 = 10,
                Field3 = 5.5m,
                OtherField1 = 200,
                OtherField2 = "NA",
                ChildBs = new List<DmChildB>
                {
                    new DmChildB()
                    {
                        Id = -2,
                        Field1 = 1000,
                        OtherField1 = "na",
                        GrandChildB1s = new List<DmGrandChildB1>()
                        {
                            new DmGrandChildB1 { Field1 = new DateTime(2000,1,1),Field2 = "F2a" },
                            new DmGrandChildB1 { Field1 = new DateTime(2001,1,1),Field2 = "F2b" }
                        },
                        GrandChildB2s = new List<DmGrandChildB2>()
                        {
                            new DmGrandChildB2 { Field1 = true }
                        }
                    },
                }
            };
            var dbParent1 = service.InsertParentB(modelParent1);

            // Get inserted graph and test
            service = serviceFactory.Create();
            var modelParent2 = service.GetDmParentB(dbParent1.Id);
            Assert.AreEqual("DmParentB:F1,10,5.50;" +
                "DmChildB:1000;" +
                "DmGrandChildB1:1/1/2000 12:00:00 AM,F2a;" +
                "DmGrandChildB1:1/1/2001 12:00:00 AM,F2b;" +
                "DmGrandChildB2:True", Helper.Serialize(modelParent2));

            // Complex graph update
            modelParent2.Field1 = null;
            modelParent2.Field2 = 11;
            modelParent2.Field3 = 6.5m;
            modelParent2.ChildBs.Add(
                new DmChildB()
                {
                    Id = -100,  // Ids <= 0 are new records.
                    Field1 = 2000,
                    OtherField1 = "",
                    GrandChildB2s = new List<DmGrandChildB2>()
                    {
                        new DmGrandChildB2 { Field1 = true },
                        new DmGrandChildB2 { Field1 = false }
                    }
                });
            var childB = modelParent2.ChildBs.ToList()[0];
            childB.GrandChildB1s.Remove(childB.GrandChildB1s.ToList()[0]);
            childB.GrandChildB1s.ToList()[0].Field1 = new DateTime(2100, 12, 31);
            childB.GrandChildB1s.Add(new DmGrandChildB1 { Field1 = new DateTime(2200, 1, 1), Field2 = "F2New" });
            var dbParent2 = service.GetParentB(modelParent2.Id);
            service.UpdateParentB(modelParent2, dbParent2);

            // Test update
            service = serviceFactory.Create();
            var modelParent3 = service.GetDmParentB(modelParent2.Id);
            Assert.AreEqual("DmParentB:,11,6.50;" +
                "DmChildB:1000;" +
                "DmGrandChildB1:12/31/2100 12:00:00 AM,F2b;" +
                "DmGrandChildB1:1/1/2200 12:00:00 AM,F2New;" +
                "DmGrandChildB2:True;" +
                "DmChildB:2000;" +
                "DmGrandChildB2:True;" +
                "DmGrandChildB2:False", Helper.Serialize(modelParent3));

            // Remove graph and test
            service = serviceFactory.Create();
            var dbParent3 = service.GetParentB(modelParent3.Id);
            service.DeleteParentB(dbParent3);
            Assert.IsNull(service.GetParentA(modelParent3.Id));
        }


        [TestMethod, Ignore]
        public void SpeedTests1()
        {
            var d1 = this.RunTestsXTimes(new ContextNoProxiesServiceFactory(Helper.CreateAutomaticFactory()), 1);

            var d2 = this.RunTestsXTimes(new ContextNoProxiesServiceFactory(Helper.CreateAutomaticFactory()), 100);

            var cachedFactory = new EntityMapperCachedFactory(Helper.CreateAutomaticFactory());
            var d3 = this.RunTestsXTimes(new ContextNoProxiesServiceFactory(cachedFactory), 100);

            cachedFactory = new EntityMapperCachedFactory(Helper.CreateCustomFactory());
            var d4 = this.RunTestsXTimes(new ContextNoProxiesServiceFactory(cachedFactory), 100);
        }

        [TestMethod, Ignore]
        public void SpeedTests2()
        {
            var d1 = this.RunBigInsertAndDelete(new ContextNoProxiesServiceFactory(Helper.CreateAutomaticFactory()));
        }

        public double RunBigInsertAndDelete(ServiceFactory serviceFactory)
        {
            var sw = new Stopwatch();
            sw.Start();

            var service = serviceFactory.Create();
            var modelParent1 = new ParentA { P1 = "SPEED", P2 = 1000, P3 = 2000 };
            for (var i = 0; i < 100; i++)
            {
                modelParent1.ChildA1s.Add(new ChildA1() { P1 = "ABC", P2 = new DateTime(2010, 1, 1), P3 = null });
                modelParent1.ChildA1s.Add(
                    new ChildA1()
                    {
                        P1 = "XYZ",
                        P2 = new DateTime(2000, 1, 1),
                        P3 = new DateTime(2001, 1, 1),
                        GrandChildA1s = new HashSet<GrandChildA1>
                            {
                                new GrandChildA1 { P1 = i, P2 = 8, P3 = true },
                                new GrandChildA1 { P1 = i, P2 = 9, P3 = null },
                            }
                    });
            }
            var dbParent1 = service.InsertParentA(modelParent1);

            service = serviceFactory.Create();
            var dbParent2 = service.GetParentA(dbParent1.Id);
            service.DeleteParentA(dbParent2);

            sw.Stop();
            return sw.ElapsedMilliseconds / 1000.0;
        }

        public double RunTestsXTimes(ServiceFactory serviceFactory, int iterations)
        {
            var sw = new Stopwatch();
            sw.Start();
            for (var i = 0; i < iterations; i++)
            {
                this.RunInsertUpdateDeleteTestsA(serviceFactory);
            }
            sw.Stop();
            return sw.ElapsedMilliseconds / 1000.0;
        }

    }
}
