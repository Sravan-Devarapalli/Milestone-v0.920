CREATE TABLE [Skills].[PersonQualification](
	[QualificationTypeId] int NOT NULL ,
	[PersonId] [int] NOT NULL,
	[Info] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonQualification] PRIMARY KEY CLUSTERED 
(
	[QualificationTypeId] ASC,
	[PersonId] ASC
)
)
