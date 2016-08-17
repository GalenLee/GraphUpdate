using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GraphUpdateDataAccess.Context;

namespace GraphUpdateTests
{
    [TestClass]
    public class AssemblyTestSetup
    {
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            using (var dbContext = new GraphUpdateContextBase())
            {
                var forceCreate = dbContext.ParentAs.Count();
            }
        }
    }
}
