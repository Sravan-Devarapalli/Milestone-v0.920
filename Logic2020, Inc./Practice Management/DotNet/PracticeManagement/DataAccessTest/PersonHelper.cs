using System;
using DataTransferObjects;

namespace DataAccessTest
{
	public static class PersonHelper
	{
		/// <summary>
		/// Makes a person that should be acceptable in an insert
		/// </summary>
		/// <returns></returns>
		public static DataTransferObjects.Person GetPerson()
		{
			DataTransferObjects.Person person = new DataTransferObjects.Person();
			person.FirstName = ("Name, " + Guid.NewGuid()).Substring(0, 40);
			person.LastName = "Last Name";
			person.Alias = Guid.NewGuid().ToString() + "@techdatasolutions.com";
			person.HireDate = DateTime.Now;
			person.Status = new PersonStatus();
			person.Status.Id = (int)PersonStatusType.Active;
			person.EmployeeNumber = Guid.NewGuid().ToString().Substring(0, 12);
			person.RoleNames = new string[0];

			return person;
		}

		/// <summary>
		/// Creates a record in the Person table.
		/// </summary>
		/// <returns>An ID of the new record.</returns>
		public static int CreateTestPerson()
		{
			using (System.Data.Linq.DataContext context =
				new System.Data.Linq.DataContext(DataAccess.Other.DataSourceHelper.DataConnection))
			{
				// Creating a Person record
				TestEntities.Person person = new TestEntities.Person();

				person.FirstName = ("Test Person" + Guid.NewGuid().ToString()).Substring(0, 40);
				person.LastName = "Test Person";
				person.Alias = Guid.NewGuid().ToString() + "@techdatasolutions.com";
				person.HireDate = DateTime.Today;
				person.PersonStatusId = (int)PersonStatusType.Active;
				person.EmployeeNumber = Guid.NewGuid().ToString().Substring(0, 12);
				
				System.Data.Linq.Table<TestEntities.Person> tblPerson =
					context.GetTable<TestEntities.Person>();
				tblPerson.InsertOnSubmit(person);

				context.SubmitChanges();

				return person.PersonId;
			}
		}

		/// <summary>
		/// Removes a test record from the database.
		/// </summary>
		/// <param name="personId">An ID of the record to be reemoved.</param>
		public static void DeleteTestPerson(int personId)
		{
			try
			{
				new System.Data.Linq.DataContext(DataAccess.Other.DataSourceHelper.DataConnection).ExecuteCommand(
					"DELETE Client FROM dbo.Client WHERE Client.DefaultSalesPersonId = " + personId.ToString() + "\n" +
					"DELETE FROM dbo.Person WHERE PersonId = " + personId.ToString() + "\n");
			}
			catch (Exception)
			{
				// Skip any exceptions here to not affect test results.
			}
		}
	}
}

