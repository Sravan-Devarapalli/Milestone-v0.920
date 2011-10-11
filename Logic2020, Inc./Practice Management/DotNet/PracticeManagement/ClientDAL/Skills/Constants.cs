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
            public const string SkillLevelId = "SkillLevelId";
            public const string SkillId = "SkillId";
            public const string SkillCategoryName = "SkillCategoryName";
            public const string SkillLevelName = "SkillLevelName";            
            public const string SkillName = "SkillName";
        }

        public class FunctionNames
        {
        }

        public class ParameterNames
        {

            public const string TenantId = "@TenantId";

        }

        public class ProcedureNames
        {
            public const string GetSkillCategoriesAll = "Skills.GetSkillCategoriesAll";
            public const string GetSkillLevelsAll = "Skills.GetSkillLevelsAll";
            public const string GetSkillsAll = "Skills.GetSkillsAll";
        }

        #endregion
    }
}

