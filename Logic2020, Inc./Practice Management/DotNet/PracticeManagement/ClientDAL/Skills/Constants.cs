using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DataAccess.Skills
{
    public class Constants
    {
        #region "Column Names"
        public class ColumnNames
        {
            public const string DisplayOrder = "DisplayOrder";
            public const string SkillCategoryId = "SkillCategoryId";
            public const string SkillTypeId = "SkillTypeId";
            public const string SkillTypeDescription = "SkillTypeDescription";
            public const string SkillLevelId = "SkillLevelId";
            public const string SkillId = "SkillId";
            public const string SkillCategoryName = "SkillCategoryName";
            public const string SkillLevelName = "SkillLevelName";
            public const string SkillLevelDefinition = "SkillLevelDefinition";
            public const string SkillName = "SkillName";
            public const string YearsExperience = "YearsExperience";
            public const string LastUsed = "LastUsed";
            public const string IndustryId = "IndustryId";
            public const string IndustryName = "IndustryName";
            public const string PersonId = "PersonId";
            public const string LastName = "LastName";
            public const string FirstName = "FirstName";
            public const string ProfileId = "ProfileId";
            public const string ProfileName = "ProfileName";
            public const string ProfileUrl = "ProfileUrl";
            public const string ModifiedBy = "ModifiedBy";
            public const string ModifiedDate = "ModifiedDate";
            public const string IsHighlighted = "IsHighlighted";
            public const string PersonPicture = "PersonPicture";
            public const string ModifiedByName = "ModifiedByName";
            public const string IsDefault = "IsDefault";
            public const string HasPicture = "HasPicture";
        }

        public class FunctionNames
        {
        }

        public class ParameterNames
        {
            public const string PersonId = "@PersonId";
            public const string Skills = "@Skills";
            public const string IndustrySkills = "@IndustrySkills";
            public const string UserLogin = "@UserLogin";
            public const string SkillsSearchXML = "@SkillsSearchXML";
            public const string ProfileUrl = "@ProfileUrl";
            public const string ProfilesXml = "@ProfilesXml";
            public const string PictureData = "@PictureData";
            public const string PictureFileName = "@PictureFileName";
        }

        public class ProcedureNames
        {
            public const string GetSkillCategoriesAll = "Skills.GetSkillCategoriesAll";
            public const string GetSkillLevelsAll = "Skills.GetSkillLevelsAll";
            public const string GetSkillTypesAll = "Skills.GetSkillTypesAll";
            public const string GetSkillsAll = "Skills.GetSkillsAll";
            public const string GetIndustrySkillsAll = "Skills.GetIndustrySkillsAll";
            public const string GetPersonSkillsByPersonId = "Skills.GetPersonSkillsByPersonId";
            public const string GetPersonIndustriesByPersonId = "Skills.GetPersonIndustriesByPersonId";
            public const string SavePersonSkills = "Skills.SavePersonSkills";
            public const string SavePersonIndustrySkills = "Skills.SavePersonIndustrySkills";
            public const string PersonsSearchBySkills = "Skills.PersonsSearchBySkills";
            public const string SavePersonProfiles = "Skills.SavePersonProfiles";
            public const string GetPersonProfiles = "Skills.GetPersonProfiles";
            public const string SavePersonPicture = "Skills.SavePersonPicture";
            public const string GetPersonWithHasPictureField = "Skills.GetPersonWithHasPictureField";
            public const string GetPersonPicture = "Skills.GetPersonPicture";
            public const string DeletePersonPicture = "Skills.DeletePersonPicture";
        }

        #endregion
    }
}

