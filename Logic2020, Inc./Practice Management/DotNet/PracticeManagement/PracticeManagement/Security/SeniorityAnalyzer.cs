using System.Collections.Generic;
using System.Web.Security;
using DataTransferObjects;

namespace PraticeManagement.Security
{
    public class SeniorityAnalyzer
    {
        private bool isAdminOrSales;
        private Person currentPerson;
        private bool greaterSeniorityExists;

        /// <summary>
        /// Init constructor of SeniorityAnalyzer.
        /// </summary>
        public SeniorityAnalyzer(Person currentPerson)
        {
            this.currentPerson = currentPerson;

            greaterSeniorityExists = false;

            isAdminOrSales =
               Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.AdministratorRoleName) ||
               Roles.IsUserInRole(DataTransferObjects.Constants.RoleNames.SalespersonRoleName);
        }

        public bool GreaterSeniorityExists
        {
            get
            {
                return greaterSeniorityExists;
            }
        }

        public bool OneWithGreaterSeniorityExists(IEnumerable<Person> persons)
        {
            if (isAdminOrSales)
                return false;

            foreach (Person person in persons)
                if (IsOtherGreater(person))
                    return true;

            return false;
        }

        public bool OneWithGreaterSeniorityExists(List<MilestonePerson> persons)
        {
            if (isAdminOrSales)
                return false;

            foreach (MilestonePerson person in persons)
                if (IsOtherGreater(person.Person))
                    return true;

            return false;
        }

        public bool IsOtherGreater(Person other)
        {
            if (isAdminOrSales || currentPerson.Id == other.Id)
                return false;

            Seniority otherSeniority = other.Seniority;
            if (otherSeniority == null)
                return false;

            bool otherHasGreaterSeniority = currentPerson.Seniority.OtherHasGreaterOrEqualSeniority(otherSeniority);
            if (otherHasGreaterSeniority)
                greaterSeniorityExists = true;

            return otherHasGreaterSeniority;
        }

        public bool IsOtherGreater(int seniorityId)
        {
            return IsOtherGreater(CreateSimplePerson(seniorityId));
        }

        /// <summary>
        /// Creates person with just seniority value
        /// </summary>
        /// <param name="seniorityId"></param>
        /// <returns></returns>
        private static Person CreateSimplePerson(int seniorityId)
        {
            Person other = new Person();
            other.Seniority = new Seniority();
            other.Seniority.Id = seniorityId;
            return other;
        }
    }
}

