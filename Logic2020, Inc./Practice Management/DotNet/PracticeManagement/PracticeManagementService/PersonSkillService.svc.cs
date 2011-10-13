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

        public void SavePersonSkills(int personId, string skillsXml)
        {
            //XmlDocument doc = new XmlDocument();
            //XmlElement root = doc.CreateElement("Skills");

            //foreach (var skill in skills)
            //{
            //    XmlElement skillTag = doc.CreateElement("Skill");

            //    skillTag.SetAttribute("Id", skill.Skill == null ? "" : skill.Skill.Id.ToString());
            //    skillTag.SetAttribute("Level", skill.SkillLevel == null ? "" : skill.SkillLevel.Id.ToString());
            //    skillTag.SetAttribute("Experience", skill.YearsExperience.HasValue ? skill.YearsExperience.Value.ToString() : "");
            //    skillTag.SetAttribute("LastUsed", skill.LastUsed == null ? "" : skill.LastUsed.ToShortDateString());

            //    root.AppendChild(skillTag);
            //}

            //doc.AppendChild(root);

            PersonSkillDAL.SavePersonSkills(personId, skillsXml);
        }

        public void SavePersonIndustrySkills(int personId, string industrySkillsXml)
        {
            PersonSkillDAL.SavePersonIndustrySkills(personId, industrySkillsXml);
        }
    }
}

