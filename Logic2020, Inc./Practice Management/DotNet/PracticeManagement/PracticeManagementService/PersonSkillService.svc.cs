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
using DataAccess;


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

        public List<SkillType> SkillTypesAll()
        {
            return PersonSkillDAL.GetSkillTypesAll();
        }

        public List<Industry> GetIndustrySkillsAll()
        {
            return PersonSkillDAL.GetIndustrySkillsAll();
        }

        public Person GetPersonProfilesWithSkills(int personId)
        {
            Person person = GetPersonWithSkills(personId);
            person.Profiles = PersonSkillDAL.GetPersonProfiles(personId);
            return person;
        }

        public List<Profile> GetPersonProfiles(int personId)
        {
            return PersonSkillDAL.GetPersonProfiles(personId);
        }

        public Person GetPersonWithSkills(int personId)
        {
            Person person = PersonSkillDAL.GetPersonWithPictureUrl(personId);
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

        public List<Person> PersonsSearchBySkills(string skillsSearchXML)
        {
            return PersonSkillDAL.PersonsSearchBySkills(skillsSearchXML);
        }

        public void SavePersonProfiles(int personId, string profilesXml, string userLogin)
        {
            PersonSkillDAL.SavePersonProfiles(personId, profilesXml, userLogin);
        }

        public void SavePersonPictureUrl(int personId, string pictureUrl, string userLogin)
        {
            PersonSkillDAL.SavePersonPictureUrl(personId, pictureUrl, userLogin);
        }
    }
}

