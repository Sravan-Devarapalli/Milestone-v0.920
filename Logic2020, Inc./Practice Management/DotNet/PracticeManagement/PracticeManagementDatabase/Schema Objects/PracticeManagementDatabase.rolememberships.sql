EXECUTE sp_addrolemember @rolename = N'aspnet_Membership_BasicAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Membership_ReportingAccess', @membername = N'aspnet_Membership_FullAccess';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Membership_ReportingAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Profile_FullAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Profile_BasicAccess', @membername = N'aspnet_Profile_FullAccess';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Profile_BasicAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Profile_ReportingAccess', @membername = N'aspnet_Profile_FullAccess';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Profile_ReportingAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Roles_FullAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Roles_BasicAccess', @membername = N'aspnet_Roles_FullAccess';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Roles_BasicAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Roles_ReportingAccess', @membername = N'aspnet_Roles_FullAccess';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Roles_ReportingAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Personalization_FullAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Personalization_BasicAccess', @membername = N'aspnet_Personalization_FullAccess';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Personalization_BasicAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Personalization_ReportingAccess', @membername = N'aspnet_Personalization_FullAccess';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Personalization_ReportingAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_WebEvent_FullAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'db_owner', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'db_accessadmin', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'db_securityadmin', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'db_ddladmin', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'db_backupoperator', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'db_datareader', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'db_datawriter', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'db_denydatareader', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'db_denydatawriter', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'PracticeManager', @membername = N'PracticeManagementUser';


GO
EXECUTE sp_addrolemember @rolename = N'PracticeManager', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'PracticeUnitTest', @membername = N'PracticeManagementUser';


GO
EXECUTE sp_addrolemember @rolename = N'PracticeUnitTest', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Membership_FullAccess', @membername = N'setuptest';


GO
EXECUTE sp_addrolemember @rolename = N'aspnet_Membership_BasicAccess', @membername = N'aspnet_Membership_FullAccess';


