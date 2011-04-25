using System;
using DataAccess;
using DataTransferObjects;
using NUnit.Framework;

namespace DataAccessTest
{
    [TestFixture]
    public class RecruiterDALTest
    {
        private static Person GetPerson()
        {
            Person person = new Person();
            person.Alias = "alias@" + Guid.NewGuid();
            person.Name = "name, " + Guid.NewGuid();
            return person;
        }

        private static void AddRecruiterRole(Person person)
        {
            person.AsRecruiter = new Recruiter();
        }
        
        [Test]
        public void InsertTest()
        {
            Person person = GetPerson();
            AddRecruiterRole(person);

            PersonDAL.PersonInsert(person);
            RecruiterDAL.Insert(person);
        }

        [Test]
        public void IsActiveRecruiterTest()
        {
            Person person = GetPerson();
            AddRecruiterRole(person);

            PersonDAL.PersonInsert(person);
            RecruiterDAL.Insert(person);

            bool isActiveRecruiter = RecruiterDAL.IsActiveRecruiter(person);
            Assert.IsTrue(isActiveRecruiter, "person should be active recruiter");
        }
        
        [Test]
        public void IsInactiveRecruiterTest()
        {
            Person person = GetPerson();
            AddRecruiterRole(person);

            PersonDAL.PersonInsert(person);
            RecruiterDAL.Insert(person);
            RecruiterDAL.Inactivate(person);

            bool isInactiveRecruiter = RecruiterDAL.IsInactiveRecruiter(person);
            Assert.IsTrue(isInactiveRecruiter, "recruiter inactivated should be found as inactive recruiter");
        }
        
        [Test]
        public void InactivateTest()
        {
            Person person = GetPerson();
            AddRecruiterRole(person);

            PersonDAL.PersonInsert(person);
            RecruiterDAL.Insert(person);
            RecruiterDAL.Inactivate(person);

            bool isActiveRecruiter = RecruiterDAL.IsActiveRecruiter(person);
            Assert.IsFalse(isActiveRecruiter, "person should be inactive recruiter");
        }
        
        [Test]
        public void ReactivateTest()
        {
            Person person = GetPerson();
            AddRecruiterRole(person);

            PersonDAL.PersonInsert(person);
            RecruiterDAL.Insert(person);
            RecruiterDAL.Inactivate(person);
            RecruiterDAL.Reactivate(person);

            bool isActiveRecruiter = RecruiterDAL.IsActiveRecruiter(person);
            Assert.IsTrue(isActiveRecruiter, "person should be inactive recruiter");
        }
    }
}
