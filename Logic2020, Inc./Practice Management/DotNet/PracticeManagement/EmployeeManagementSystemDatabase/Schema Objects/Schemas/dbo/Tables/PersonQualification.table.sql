CREATE TABLE [dbo].[PersonQualification](
	[QualificationTypeId] int NOT NULL ,
	[PersonId] [bigint] NOT NULL,
	[Info] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonQualification] PRIMARY KEY CLUSTERED 
(
	[QualificationTypeId] ASC,
	[PersonId] ASC
)
)
