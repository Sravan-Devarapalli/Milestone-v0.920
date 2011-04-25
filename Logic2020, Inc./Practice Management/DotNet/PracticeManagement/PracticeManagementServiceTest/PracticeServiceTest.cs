using DataTransferObjects;
using NUnit.Framework;
using PracticeManagementServiceTest.PracticeClient;

namespace PracticeManagementServiceTest
{
    [TestFixture]
    public class PracticeServiceTest
    {
        [Test]
        public void GetPracticeListTest()
        {
            PracticeServiceClient serviceClient = new PracticeServiceClient();

            Practice[] list = serviceClient.GetPracticeList();
            int found = 0;
            for (int i = 0; i < list.Length; i++)
            {
                Practice practice = list[i];
                if (practice.Id == 1 && practice.Name == "Offshore")
                {
                    ++found;
                }
                if (practice.Id == 2 && practice.Name == "Management Consulting")
                {
                    ++found;
                }
                if (practice.Id == 3 && practice.Name == "Technology Consulting")
                {
                    ++found;
                }
                if (practice.Id == 4 && practice.Name == "Administration")
                {
                    ++found;
                }
            }
            Assert.AreEqual(4, found, "All practices should be found");
        }

        [Test]
        public void GetPracticeListCountTest()
        {
            PracticeServiceClient serviceClient = new PracticeServiceClient();
            Practice[] list = serviceClient.GetPracticeList();
            Assert.AreEqual(4, list.Length, "There should be four practices");
        }
    }
}

