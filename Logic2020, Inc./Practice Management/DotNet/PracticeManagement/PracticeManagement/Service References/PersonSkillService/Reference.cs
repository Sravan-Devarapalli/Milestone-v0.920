﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.225
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PraticeManagement.PersonSkillService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PersonSkillService.IPersonSkillService")]
    public interface IPersonSkillService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonSkillService/SkillCategoriesAll", ReplyAction="http://tempuri.org/IPersonSkillService/SkillCategoriesAllResponse")]
        DataTransferObjects.Skills.SkillCategory[] SkillCategoriesAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonSkillService/SkillsAll", ReplyAction="http://tempuri.org/IPersonSkillService/SkillsAllResponse")]
        DataTransferObjects.Skills.Skill[] SkillsAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonSkillService/SkillLevelsAll", ReplyAction="http://tempuri.org/IPersonSkillService/SkillLevelsAllResponse")]
        DataTransferObjects.Skills.SkillLevel[] SkillLevelsAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonSkillService/GetIndustrySkillsAll", ReplyAction="http://tempuri.org/IPersonSkillService/GetIndustrySkillsAllResponse")]
        DataTransferObjects.Skills.Industry[] GetIndustrySkillsAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonSkillService/GetPersonWithSkills", ReplyAction="http://tempuri.org/IPersonSkillService/GetPersonWithSkillsResponse")]
        DataTransferObjects.Person GetPersonWithSkills(int personId);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonSkillService/SavePersonSkills", ReplyAction="http://tempuri.org/IPersonSkillService/SavePersonSkillsResponse")]
        void SavePersonSkills(int personId, string skillsXml, string userLogin);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IPersonSkillService/SavePersonIndustrySkills", ReplyAction="http://tempuri.org/IPersonSkillService/SavePersonIndustrySkillsResponse")]
        void SavePersonIndustrySkills(int personId, string industrySkillsXml, string userLogin);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IPersonSkillServiceChannel : PraticeManagement.PersonSkillService.IPersonSkillService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PersonSkillServiceClient : System.ServiceModel.ClientBase<PraticeManagement.PersonSkillService.IPersonSkillService>, PraticeManagement.PersonSkillService.IPersonSkillService {
        
        public PersonSkillServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public PersonSkillServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PersonSkillServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public PersonSkillServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public DataTransferObjects.Skills.SkillCategory[] SkillCategoriesAll() {
            return base.Channel.SkillCategoriesAll();
        }
        
        public DataTransferObjects.Skills.Skill[] SkillsAll() {
            return base.Channel.SkillsAll();
        }
        
        public DataTransferObjects.Skills.SkillLevel[] SkillLevelsAll() {
            return base.Channel.SkillLevelsAll();
        }
        
        public DataTransferObjects.Skills.Industry[] GetIndustrySkillsAll() {
            return base.Channel.GetIndustrySkillsAll();
        }
        
        public DataTransferObjects.Person GetPersonWithSkills(int personId) {
            return base.Channel.GetPersonWithSkills(personId);
        }
        
        public void SavePersonSkills(int personId, string skillsXml, string userLogin) {
            base.Channel.SavePersonSkills(personId, skillsXml, userLogin);
        }
        
        public void SavePersonIndustrySkills(int personId, string industrySkillsXml, string userLogin) {
            base.Channel.SavePersonIndustrySkills(personId, industrySkillsXml, userLogin);
        }
    }
}

