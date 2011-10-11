﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataTransferObjects.Skills;
using DataAccess.Skills;
using System.ServiceModel.Activation;

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
    }
}

