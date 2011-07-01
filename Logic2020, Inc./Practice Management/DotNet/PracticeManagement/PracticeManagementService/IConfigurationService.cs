using System.Collections.Generic;
using System.ServiceModel;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;
using System;

namespace PracticeManagementService
{
    [ServiceContract]
    public interface IConfigurationService
    {
        [OperationContract]
        List<EmailTemplate> GetAllEmailTemplates();

        [OperationContract]
        EmailTemplate EmailTemplateGetByName(string emailTemplateName);

        [OperationContract]
        bool UpdateEmailTemplate(EmailTemplate template);

        [OperationContract]
        bool AddEmailTemplate(EmailTemplate template);

        [OperationContract]
        bool DeleteEmailTemplate(int templateId);

        [OperationContract]
        void CheckProjectedProjects(int templateId, string companyName);

        [OperationContract]
        EmailData GetEmailData(EmailContext emailContext);

        [OperationContract]
        void CheckProjectedProjectsByHireDate(int templateId, string companyName);

        [OperationContract]
        CompanyLogo GetCompanyLogoData();

        [OperationContract]
        void SaveCompanyLogoData(string title, string imagename, string imagePath, Byte[] data);

        [OperationContract]
        void SaveResourceKeyValuePairs(SettingsType settingType, Dictionary<string, string> dictionary);


        [OperationContract]
        bool SaveResourceKeyValuePairItem(SettingsType settingType, string key, string value);

        [OperationContract]
        Dictionary<string, string> GetResourceKeyValuePairs(SettingsType settingType);

        [OperationContract]
        Boolean VerifySMTPSettings(string MailServer, int PortNumber, bool EnableSSl, string UserName, string Password, string PMSupportEmailAddress);

        [OperationContract]
        void SaveMarginInfoDetail(List<Triple<DefaultGoalType, Triple<SettingsType, string, string>, List<ClientMarginColorInfo>>> marginInfoList);

        [OperationContract]
        List<ClientMarginColorInfo> GetMarginColorInfoDefaults(DefaultGoalType goalType);
    }
}

