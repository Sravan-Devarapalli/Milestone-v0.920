using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataTransferObjects.Skills;
using DataAccess.Skills;
using System.ServiceModel.Activation;
using DataTransferObjects;


namespace PracticeManagementService
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class PersonSkillService : IPersonSkillService
    {
        public List<SkillCategory> SkillCategoriesAll()
        {
            return PersonSkillDAL.GetSkillCategoriesAll();
        }

        public List<Skill> SkillsAll()
        {
            return PersonSkillDAL.GetSkillsAll();
        }

        public List<SkillLevel> SkillLevelsAll()
        {
            return PersonSkillDAL.GetSkillLevelsAll();
        }

        public List<Industry> GetIndustrySkillsAll()
        {
            return PersonSkillDAL.GetIndustrySkillsAll();
        }

        public Person GetPersonWithSkills(int personId)
        {
            var person = new Person()
            {
                Id = personId
            };

            person.Skills = PersonSkillDAL.GetPersonSkillsByPersonId(personId);
            person.Industries = PersonSkillDAL.GetPersonIndustriesByPersonId(personId);

            return person;
        }

        public void SavePersonSkills(int personId, string skillsXml, string userLogin)
        {
            PersonSkillDAL.SavePersonSkills(personId, skillsXml, userLogin);
        }

        public void SavePersonIndustrySkills(int personId, string industrySkillsXml, string userLogin)
        {
            PersonSkillDAL.SavePersonIndustrySkills(personId, industrySkillsXml, userLogin);
        }

        public List<Person> PersonsSearchBySkillsText(string skillsSearchText)
        {
            return PersonSkillDAL.PersonsSearchBySkillsText(skillsSearchText);
        }
    }
}

