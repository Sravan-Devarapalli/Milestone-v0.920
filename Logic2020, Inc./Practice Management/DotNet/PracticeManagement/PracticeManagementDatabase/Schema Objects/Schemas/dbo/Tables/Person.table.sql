CREATE TABLE [dbo].[Person] (
    [PersonId]         INT            IDENTITY (1, 1) NOT NULL,
    [PTODaysPerAnnum]  INT            NOT NULL,
    [HireDate]         DATETIME       NOT NULL,
    [TerminationDate]  DATETIME       NULL,
    [Alias]            NVARCHAR (100) NULL,
    [DefaultPractice]  INT            NULL,
    [FirstName]        NVARCHAR (40)  NOT NULL,
    [LastName]         NVARCHAR (40)  NOT NULL,
    [Notes]            NVARCHAR (MAX) NULL,
    [PersonStatusId]   INT            NOT NULL,
    [EmployeeNumber]   NVARCHAR (12)  NOT NULL,
    [SeniorityId]      INT            NULL,
    [ManagerId]        INT            NULL,
    [PracticeOwnedId]  INT            NULL,
    [IsDefaultManager] BIT            NOT NULL,
    [TelephoneNumber]  NCHAR (20)     NULL
);


