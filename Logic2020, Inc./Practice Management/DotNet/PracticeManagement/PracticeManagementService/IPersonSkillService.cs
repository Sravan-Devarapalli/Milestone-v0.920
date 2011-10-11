using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using DataTransferObjects.Skills;

namespace PracticeManagementService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IPersonSkillService" in both code and config file together.
    [ServiceContract]
    public interface IPersonSkillService
    {

        //[OperationContract]
        //List<SkillType> GetSkillStatuses();

        [OperationContract]
        List<SkillCategory> SkillCategoriesAll();

        [OperationContract]
        List<Skill> SkillsAll();

        [OperationContract]
        List<SkillLevel> SkillLevelsAll();

        //[OperationContract]
        //List<PersonSkill> PersonSkillsGetBySkillCategory(int SkillCategoryId);

    }
}

