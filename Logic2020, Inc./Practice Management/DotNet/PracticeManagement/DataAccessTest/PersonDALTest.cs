using System;
using System.Collections.Generic;
using DataAccess;
using DataTransferObjects;
using NUnit.Framework;

namespace DataAccessTest
{
    [TestFixture]
    public class PersonDALTest
    {
		private int testPersonId;
		private List<int> createdIds;

		/// <summary>
		/// Initializes the Person Access tests.
		/// </summary>
		[TestFixtureSetUp]
		public void Initializer()
		{
			testPersonId = PersonHelper.CreateTestPerson();

			createdIds = new List<int>();
			createdIds.Add(testPersonId);
		}

		/// <summary>
		/// Finalizes the Person Access tests.
		/// </summary>
		[TestFixtureTearDown]
		public void Finalizer()
		{
			foreach (int id in createdIds)
			{
				PersonHelper.DeleteTestPerson(id);
			}
		}

        [Test]
        public void PersonInactivateTest()
        {
			Person person = null;
            const int someNumber = 1;
            for (int i = 0; i < someNumber; ++i)
            {
				person = PersonHelper.GetPerson();
				PersonDAL.PersonInsert(person, "Admin@logic2020.com");

				Assert.IsTrue(person.Id.HasValue,
					"The PersonDAL.PersonInsert didn't return an expected Person ID.");
				createdIds.Add(person.Id.Value);
            }
            List<Person> list = PersonDAL.PersonListAll();
            int firstCount = list.Count;

			Person firstPerson = person;
			PersonDAL.PersonInactivate(firstPerson);

            list = PersonDAL.PersonListAll();
            Assert.GreaterOrEqual(firstCount - 1,
				list.Count,
				"Inactivating one person should cause active list size to decrease");
        }

        [Test]
        [Description("duplicate aliases should not be allowed")]
		[ExpectedException(typeof(DataAccessException))]
        public void PersonInsertAliasCandidateKeyTest()
        {
            Person person = new Person();
            person.FirstName = "a" + DateTime.Now;
            person.Alias = "a" + DateTime.Now;
			PersonDAL.PersonInsert(person, "Admin@logic2020.com");

			Assert.IsTrue(person.Id.HasValue,
				"The PersonDAL.PersonInsert didn't return an expected Person ID.");
			createdIds.Add(person.Id.Value);

            person.FirstName = "b" + DateTime.Now;
			PersonDAL.PersonInsert(person, "Admin@logic2020.com");

			Assert.IsTrue(person.Id.HasValue,
				"The PersonDAL.PersonInsert didn't return an expected Person ID.");
			createdIds.Add(person.Id.Value);
        }

		[Test]
		[Description("duplicate names should not be allowed")]
		[ExpectedException(typeof(DataAccessException))]
		public void PersonInsertNameCandidateKeyTest()
		{
			Person person = new Person();
			person.FirstName = "a";
			person.Alias = "a";
			PersonDAL.PersonInsert(person, "Admin@logic2020.com");

			Assert.IsTrue(person.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
			createdIds.Add(person.Id.Value);

			person.Alias = "b";
			PersonDAL.PersonInsert(person, "Admin@logic2020.com");

			Assert.IsTrue(person.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
			createdIds.Add(person.Id.Value);
		}

		[Test]
		[ExpectedException(typeof (DataAccessException))]
		[Description("Data strore requires a non-null name")]
		public void PersonInsertNullAliasTest()
		{
			Person person = new Person();
			person.FirstName = "";
			PersonDAL.PersonInsert(person, "Admin@logic2020.com");

			Assert.IsTrue(person.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
			createdIds.Add(person.Id.Value);
		}

		[Test]
		[ExpectedException(typeof(DataAccessException))]
		[Description("Data stroe requires a non-null name")]
		public void PersonInsertNullNameTest()
		{
			Person person = new Person();
			PersonDAL.PersonInsert(person, "Admin@logic2020.com");

			Assert.IsTrue(person.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
			createdIds.Add(person.Id.Value);
		}

		[Test]
		public void PersonInsertTest()
		{
			Person person = PersonHelper.GetPerson();
			PersonDAL.PersonInsert(person, "Admin@logic2020.com");

			Assert.IsTrue(person.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
			createdIds.Add(person.Id.Value);

			Assert.IsNotNull(person.Id, "System should assign an id");
		}

        [Test]
        public void PersonListAllTest()
        {
            const int someNumber = 10;
            for (int i = 0; i < someNumber; ++i)
            {
				Person person = PersonHelper.GetPerson();
				PersonDAL.PersonInsert(person, "Admin@logic2020.com");

				Assert.IsTrue(person.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
				createdIds.Add(person.Id.Value);
            }
            List<Person> list = PersonDAL.PersonListAll();
            int firstCount = list.Count;
            Assert.GreaterOrEqual(firstCount, someNumber, "There should be at least {0} persons", someNumber);
            for (int i = 0; i < someNumber; ++i)
            {
				Person person = PersonHelper.GetPerson();
				PersonDAL.PersonInsert(person, "Admin@logic2020.com");

				Assert.IsTrue(person.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
				createdIds.Add(person.Id.Value);
            }
            list = PersonDAL.PersonListAll();
            Assert.AreEqual(firstCount+someNumber, list.Count, "should be keeping track of insert counts");
        }

		/// <summary>
		/// A test for the PersonDAL.GetById(int)
		/// </summary>
		[Test(Description = "A test for the PersonDAL.GetById(int)")]
		public void GetByIdTest()
		{
			Person person = PersonDAL.GetById(testPersonId);

			Assert.IsTrue(person.Id.HasValue, "The PersonDAL.GetById didn't return an expected Person ID.");
			Assert.AreEqual(person.Id.Value, testPersonId, "The PersonDAL.GetById didn't return an expected value.");
		}

        [Test]
        public void PersonListFilteredActiveTest()
        {
			Person person = null;
            const int someNumber = 1;
            for (int i = 0; i < someNumber; ++i)
            {
				person = PersonHelper.GetPerson();
				PersonDAL.PersonInsert(person, "Admin@logic2020.com");

				Assert.IsTrue(person.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
				createdIds.Add(person.Id.Value);
            }
            List<Person> list = PersonDAL.PersonListAll();
            int firstCount = list.Count;

			Person firstPerson = person;
            PersonDAL.PersonInactivate(firstPerson);

            list = PersonDAL.PersonListAll();
            Assert.AreEqual(firstCount-1, list.Count, "Inactivating one person should cause active list size to decrease");

            List<Person> allList = PersonDAL.PersonListFiltered(null, true, 0, 0, null, null);
            Assert.GreaterOrEqual(allList.Count, firstCount, "Inactivating should not impact the count for 'showAll'");
        }

        [Test]
        [Ignore("TBD this is deferred until we have persons that have practices, i.e. Consultants")]
        public void PersonListFilteredPracticeTest()
        {
        }

        [Test]
        public void PersonReactivateTest()
        {
            const int someNumber = 10;
            for (int i = 0; i < someNumber; ++i)
            {
				Person person = PersonHelper.GetPerson();
				PersonDAL.PersonInsert(person, "Admin@logic2020.com");

				Assert.IsTrue(person.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
				createdIds.Add(person.Id.Value);
            }
            List<Person> list = PersonDAL.PersonListAll();
            int firstCount = list.Count;

			Person firstPerson = PersonDAL.GetById(testPersonId);
            PersonDAL.PersonInactivate(firstPerson);

            list = PersonDAL.PersonListAll();
            Assert.AreEqual(firstCount - 1, list.Count, "Inactivating one person should cause active list size to decrease");

            PersonDAL.PersonReactivate(firstPerson);
            list = PersonDAL.PersonListAll();
            Assert.AreEqual(firstCount, list.Count);
        }

        [Test]
        public void PersonUpdateTest()
        {
			Person firstPerson = PersonDAL.GetById(testPersonId);

			Assert.IsTrue(firstPerson.Id.HasValue, "The PersonDAL.PersonInsert didn't return an expected Person ID.");
			createdIds.Add(firstPerson.Id.Value);
            int firstId = firstPerson.Id.Value;

			Person secondPerson = PersonHelper.GetPerson();
            secondPerson.Id = firstId;
			PersonDAL.PersonUpdate(secondPerson, "Admin@logic2020.com");

            List<Person> list = PersonDAL.PersonListAll();
            foreach (Person person in list)
            {
                if (person.Id == firstId)
                {
                    Assert.AreEqual(secondPerson.Alias, person.Alias, "alias should be updated");
                    TimeSpan hireDiff = secondPerson.HireDate.Subtract(person.HireDate);
                    Assert.LessOrEqual(hireDiff.TotalMinutes, 1, "Hire date shuold be updated");

                    if (secondPerson.TerminationDate.HasValue)
                    {
                        TimeSpan termDiff = secondPerson.TerminationDate.Value.Subtract(person.TerminationDate.Value);
                        Assert.LessOrEqual(termDiff.TotalMinutes, 1, "Term date should be updated");
                    }

					// TODO: [upcoming] Use Person.Status instead of Person.Inactive
					//Assert.AreEqual(secondPerson.Inactive, person.Inactive, "inactive should be updated");
                    Assert.AreEqual(secondPerson.FirstName, person.FirstName, "Name should be updated");
                    Assert.AreEqual(secondPerson.PtoDays, person.PtoDays, "PtoDays should be updated");
                }
            }
        }
    }
}
