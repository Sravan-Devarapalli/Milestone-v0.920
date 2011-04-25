using System.Collections.Generic;
using DataTransferObjects;
using NUnit.Framework;
using PracticeManagementServiceTest.PersonClient;

namespace PracticeManagementServiceTest
{
    [TestFixture]
    public class PersonServiceTest
    {
		private const string AdminLogin = "Admin@logic2020.com";

		private int testPersonId;
		private int testInactivePersonId;
		private int testActivePersonId;
		private List<int> createdIds;

		/// <summary>
		/// Initializes the Person Services tests.
		/// </summary>
		[TestFixtureSetUp]
		public void Initializer()
		{
			// Creating the test records.
			testPersonId = DataAccessTest.PersonHelper.CreateTestPerson();
			testInactivePersonId = DataAccessTest.PersonHelper.CreateTestPerson();
			testActivePersonId = DataAccessTest.PersonHelper.CreateTestPerson();

			createdIds = new List<int>();
			createdIds.Add(testPersonId);
			createdIds.Add(testInactivePersonId);
			createdIds.Add(testActivePersonId);
		}

		/// <summary>
		/// Finalizes the Person Services tests.
		/// </summary>
		[TestFixtureTearDown]
		public void Finalizer()
		{
			foreach (int id in createdIds)
			{
				DataAccessTest.PersonHelper.DeleteTestPerson(id);
			}
		}

        [Test]
		[Ignore("Temporary ignored because of performance problem.")]
		public void GetPersonDetailTest()
        {
            PersonServiceClient serviceClient = new PersonServiceClient();
            Person[] list = serviceClient.GetPersonList(null, false, 0, 0, string.Empty, null, AdminLogin);

            for (int i = 0; i < list.Length; i++)
            {
                Person listPerson = list[i];
                Person singlePerson = serviceClient.GetPersonDetail(listPerson.Id.Value);
               
                Assert.AreEqual(listPerson.Id, singlePerson.Id, "Id should be preserved");
				// TODO: [upcoming] Use Person.Status instead of Person.Inactive
				// Assert.AreEqual(listPerson.Inactive, singlePerson.Inactive, "Inactive should be preserved"); // Task #385
                Assert.AreEqual(listPerson.HireDate, singlePerson.HireDate, "HireDate should be preserved");
                Assert.AreEqual(listPerson.Alias, singlePerson.Alias, "Alias should be preserved");
                Assert.AreEqual(listPerson.FirstName, singlePerson.FirstName, "Name should be preserved");
                Assert.AreEqual(listPerson.PtoDays, singlePerson.PtoDays, "PtoDays should be preserved");
                Assert.AreEqual(listPerson.TerminationDate,
					singlePerson.TerminationDate,
					"TerminationDate should be preserved");
            }
        }

        [Test]
		[Ignore("Temporary ignored because of performance problem.")]
		public void GetPersonListTest()
        {
            PersonServiceClient serviceClient = new PersonServiceClient();
            Person[] list = serviceClient.GetPersonList(null, false, 0, 0, string.Empty, null, AdminLogin);

            IDictionary<string, int> names = new Dictionary<string, int>(list.Length);
            IDictionary<int?, int> ids = new Dictionary<int?, int>(list.Length);
            for (int i = 0; i < list.Length; i++)
            {
                Person person = list[i];

				Assert.IsFalse(ids.ContainsKey(person.Id), "id {0} should not be duplicated", person.Id);
                ids.Add(person.Id, 1);
            }
        }

		/// <summary>
		/// A test for the PersonServiceClient.SavePersonDetail(Person) method.
		/// </summary>
        [Test(Description = "A test for the PersonServiceClient.SavePersonDetail(Person) method.")]
		[Ignore("Temporary ignored because of performance problem.")]
		public void SavePersonDetailInsertTest()
        {
			Person person = DataAccessTest.PersonHelper.GetPerson();
			PersonServiceClient serviceClient = new PersonServiceClient();

			serviceClient.SavePersonDetail(person, AdminLogin);
        }

		/// <summary>
		/// A test for the PersonServiceClient.SavePersonDetail(Person) method.
		/// </summary>
        [Test(Description = "A test for the PersonServiceClient.SavePersonDetail(Person) method.")]
		[Ignore("Temporary ignored because of performance problem.")]
		public void SavePersonDetailUpdateTest()
		{
			PersonServiceClient serviceClient = new PersonServiceClient();
			Person person = serviceClient.GetPersonDetail(testPersonId);

			person.FirstName = "Test 234" + person.FirstName;

			serviceClient.SavePersonDetail(person, AdminLogin);
		}

		/// <summary>
		/// A test for the PersonServiceClient.SavePersonDetail(Person) method.
		/// </summary>
		[Test(Description = "A test for the PersonServiceClient.SavePersonDetail(Person) method.")]
		[Ignore("Temporary ignored because of performance problem.")]
		public void SavePersonActivationTest()
		{
			PersonServiceClient serviceClient = new PersonServiceClient();
			Person person = serviceClient.GetPersonDetail(testInactivePersonId);
			person.CurrentPay = null;

			// Deactivation the person
			// TODO: [upcoming] Use Person.Status instead of Person.Inactive
			//person.Inactive = true; 
			serviceClient.SavePersonDetail(person, AdminLogin);

			Person inactivePerson = serviceClient.GetPersonDetail(testInactivePersonId);
			// TODO: [upcoming] Use Person.Status instead of Person.Inactive
			//Assert.IsTrue(inactivePerson.Inactive, "The PersonServiceClient.SavePersonDetail doesn't deactivate the person."); // Task #385

			inactivePerson.CurrentPay = null;
			// Reactivating the person
			// TODO: [upcoming] Use Person.Status instead of Person.Inactive
			// inactivePerson.Inactive = false; // Task #385
			serviceClient.SavePersonDetail(inactivePerson, AdminLogin);

			Person activePerson = serviceClient.GetPersonDetail(testInactivePersonId);
			// TODO: [upcoming] Use Person.Status instead of Person.Inactive
			// Assert.IsFalse(activePerson.Inactive, "The PersonServiceClient.SavePersonDetail doesn't activate the person."); // Task #385
		}

		/// <summary>
		/// A test for the PersonServiceClient.PersonInactivate and PersonServiceClient.PersonReactivate methods.
		/// </summary>
		[Test(Description = "A test for the PersonServiceClient.PersonInactivate and PersonServiceClient.PersonReactivate methods.")]
		[Ignore("Temporary ignored because of performance problem.")]
		public void PersonActivationTest()
		{
			PersonServiceClient serviceClient = new PersonServiceClient();
			Person person = serviceClient.GetPersonDetail(testActivePersonId);

			serviceClient.PersonInactivate(person);
			Person inactivePerson = serviceClient.GetPersonDetail(testActivePersonId);
			// TODO: [upcoming] Use Person.Status instead of Person.Inactive
			//Assert.IsTrue(inactivePerson.Inactive,
			//	"The PersonServiceClient.PersonInactivate doesn't deactivate the person.");

			serviceClient.PersonReactivate(person);
			Person activePerson = serviceClient.GetPersonDetail(testActivePersonId);
			// TODO: [upcoming] Use Person.Status instead of Person.Inactive
			//Assert.IsFalse(activePerson.Inactive, 
			//	"The PersonServiceClient.PersonReactivate doesn't activate the person.");
		}
	}
}
