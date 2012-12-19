using System;
using System.Collections.Generic;
using System.Net.Mail;
using DataAccess;
using DataTransferObjects;
using DataTransferObjects.ContextObjects;

namespace PracticeManagementService
{
    [System.ServiceModel.Activation.AspNetCompatibilityRequirements(RequirementsMode = System.ServiceModel.Activation.AspNetCompatibilityRequirementsMode.Allowed)]
    public class ConfigurationService : IConfigurationService
    {
        #region "IConfigurationService Members"

        public List<EmailTemplate> GetAllEmailTemplates()
        {
            return EmailTemplateDAL.GetAllEmailTemplates();
        }

        public EmailTemplate EmailTemplateGetByName(string emailTemplateName)
        {
            return EmailTemplateDAL.EmailTemplateGetByName(emailTemplateName);
        }

        public bool UpdateEmailTemplate(EmailTemplate template)
        {
            return EmailTemplateDAL.UpdateEmailTemplate(template);
        }

        public bool AddEmailTemplate(EmailTemplate template)
        {
            return EmailTemplateDAL.AddEmailTemplate(template);
        }

        public bool DeleteEmailTemplate(int templateId)
        {
            return EmailTemplateDAL.DeleteEmailTemplate(templateId);
        }

        public EmailData GetEmailData(EmailContext emailContext)
        {
            return EmailTemplateDAL.GetEmailData(emailContext);
        }

        public CompanyLogo GetCompanyLogoData()
        {
            return ConfigurationDAL.GetCompanyLogoData();
        }

        public void SaveCompanyLogoData(string title, string imagename, string imagePath, byte[] data)
        {
            ConfigurationDAL.SaveCompanyLogoData(title, imagename, imagePath, data);
        }

        public List<string> GetAllDomains()
        {
            return ConfigurationDAL.GetAllDomains();
        }

        #endregion "IConfigurationService Members"

        public void SaveResourceKeyValuePairs(SettingsType settingType, Dictionary<string, string> dictionary)
        {
            ConfigurationDAL.SaveResourceKeyValuePairs(settingType, dictionary);
        }

        public bool SaveResourceKeyValuePairItem(SettingsType settingType, string key, string value)
        {
            SettingsHelper.SaveResourceValueToCache(settingType, key, value);
            return ConfigurationDAL.SaveResourceKeyValuePairItem(settingType, key, value);
        }

        public Dictionary<string, string> GetResourceKeyValuePairs(SettingsType settingType)
        {
            return ConfigurationDAL.GetResourceKeyValuePairs(settingType);
        }

        public bool VerifySMTPSettings(string mailServer, int portNumber, bool sSLEnabled, string userName, string password, string pMSupportEmail)
        {
            try
            {
                return MailUtil.VerifySMTPSettings(mailServer, portNumber, sSLEnabled, userName, password, pMSupportEmail);
            }
            catch (SmtpException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SaveMarginInfoDetail(List<Triple<DefaultGoalType, Triple<SettingsType, string, string>, List<ClientMarginColorInfo>>> marginInfoList)
        {
            ConfigurationDAL.SaveMarginInfoDetail(marginInfoList);
        }

        public void SavePracticesIsNotesRequiredDetails(string isNotesRequiredPracticeIdsList, string isNotesExemptedPracticeIdsList)
        {
            ConfigurationDAL.SavePracticesIsNotesRequiredDetails(isNotesRequiredPracticeIdsList, isNotesExemptedPracticeIdsList);
        }

        public List<ClientMarginColorInfo> GetMarginColorInfoDefaults(DefaultGoalType goalType)
        {
            return ConfigurationDAL.GetMarginColorInfoDefaults(goalType);
        }

        public void SaveQuickLinksForDashBoard(string linkNameList, string virtualPathList, DashBoardType dashBoardType)
        {
            ConfigurationDAL.SaveQuickLinksForDashBoard(linkNameList, virtualPathList, dashBoardType);
        }

        public List<QuickLinks> GetQuickLinksByDashBoardType(DashBoardType dashBoardtype)
        {
            return ConfigurationDAL.GetQuickLinksByDashBoardType(dashBoardtype);
        }

        public void DeleteQuickLinkById(int id)
        {
            ConfigurationDAL.DeleteQuickLinkById(id);
        }

        public void SaveAnnouncement(string text, string richText)
        {
            ConfigurationDAL.SaveAnnouncement(text, richText);
        }

        public string GetLatestAnnouncement()
        {
            return ConfigurationDAL.GetLatestAnnouncement();
        }
    }
}

