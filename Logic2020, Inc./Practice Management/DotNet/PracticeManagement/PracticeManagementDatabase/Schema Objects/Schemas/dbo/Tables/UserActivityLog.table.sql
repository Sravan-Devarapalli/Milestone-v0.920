CREATE TABLE [dbo].[UserActivityLog] (
    [ActivityID]      INT            IDENTITY (1, 1) NOT NULL,
    [ActivityTypeID]  INT            NOT NULL,
    [SessionID]       INT            NOT NULL,
    [LogDate]         DATETIME       NOT NULL,
    [SystemUser]      NVARCHAR (255) NOT NULL,
    [Workstation]     NVARCHAR (128) NULL,
    [ApplicationName] NVARCHAR (128) NULL,
    [UserLogin]       NVARCHAR (255) NULL,
    [PersonID]        INT            NULL,
    [LastName]        NVARCHAR (100) NULL,
    [FirstName]       NVARCHAR (100) NULL,
    [LogData]         XML            NULL,
	[Data]			  NVARCHAR(MAX)  NULL
);


