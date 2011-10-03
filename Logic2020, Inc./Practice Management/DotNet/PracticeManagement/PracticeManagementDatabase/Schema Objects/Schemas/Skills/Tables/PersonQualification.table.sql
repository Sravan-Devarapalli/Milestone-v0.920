CREATE TABLE [Skills].[PersonQualification](
	[QualificationTypeId] int NOT NULL ,
	[PersonId] [bigint] NOT NULL,
	[Info] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModificationDate] [datetime] NOT NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_PersonQualification] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[QualificationTypeId] ASC,
	[PersonId] ASC
)
)
