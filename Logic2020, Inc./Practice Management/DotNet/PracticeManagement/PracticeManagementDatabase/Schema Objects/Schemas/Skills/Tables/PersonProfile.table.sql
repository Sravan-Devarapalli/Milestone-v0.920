CREATE TABLE [Skills].[PersonProfile]
(
	Id				  INT   IDENTITY (1, 1) NOT NULL,
	PersonId          INT					NOT NULL,
	ProfileName		  NVARCHAR (50)			NOT NULL,
	ProfileUrl		  NVARCHAR (MAX)		NOT	NULL,
	IsDefault		  BIT					NOT NULL CONSTRAINT DF_PersonProfile_IsDefault DEFAULT 0,
	ModifiedBy        INT					NOT NULL,
	CreatedDate	      DATETIME				NOT	NULL,
	ModifiedDate	  DATETIME				NOT	NULL,
	CONSTRAINT PK_PersonProfile_Id PRIMARY KEY CLUSTERED (Id ASC)
)

