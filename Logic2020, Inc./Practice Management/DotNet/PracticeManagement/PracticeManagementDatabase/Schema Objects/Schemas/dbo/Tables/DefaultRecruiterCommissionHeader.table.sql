CREATE TABLE [dbo].[DefaultRecruiterCommissionHeader] (
    [DefaultRecruiterCommissionHeaderId] INT      IDENTITY (1, 1) NOT NULL,
    [PersonId]                           INT      NOT NULL,
    [StartDate]                          DATETIME NOT NULL,
    [EndDate]                            DATETIME NOT NULL
);


