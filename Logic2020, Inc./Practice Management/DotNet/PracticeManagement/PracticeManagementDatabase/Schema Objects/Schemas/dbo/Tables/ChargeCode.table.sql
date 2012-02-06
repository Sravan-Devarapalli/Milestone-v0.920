CREATE TABLE [dbo].[ChargeCode]
(
    Id                  INT	IDENTITY (1, 1) NOT NULL,
    ClientId            INT	NOT NULL,
    ProjectGroupId      INT	NOT NULL,
    ProjectId           INT	NOT NULL,
    PhaseId             INT	NOT NULL,
    TimeTypeId          INT	NOT NULL,
	TimeEntrySectionId	INT	NOT NULL,
	CONSTRAINT PK_ChargeCode_Id PRIMARY KEY CLUSTERED (Id ASC)
);

