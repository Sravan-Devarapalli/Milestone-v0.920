using System.Collections.Generic;
using DataAccess;
using DataTransferObjects;
using NUnit.Framework;

namespace DataAccessTest
{
    [TestFixture]
    public class PracticeDALTest
    {
        [Test]
        public void PracticeListAllTest()
        {
            List<Practice> list = PracticeDAL.PracticeListAll();
            Assert.AreEqual(4, list.Count);
        }
    }
}
