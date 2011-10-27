﻿using System;
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
            public const string SkillsSearchText = "@SkillsSearchText";            
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
            public const string PersonsSearchBySkillsText = "Skills.PersonsSearchBySkillsText";
        }

        #endregion
    }
}

