CREATE TABLE [dbo].[RecruiterCommissionHistory]
(
	[Id]			 INT IDENTITY(1,1) NOT NULL,
    [RecruitId]      INT             NOT NULL,
    [RecruiterId]    INT             NOT NULL,
    [HoursToCollect] INT             NOT NULL,
    [Amount]         DECIMAL (18, 2) NULL,
	[StartDate]		 DATETIME NOT NULL,
	[EndDate]		 DATETIME NULL,
	CONSTRAINT PK_RecruiterCommissionHistory PRIMARY KEY ([Id])
)
