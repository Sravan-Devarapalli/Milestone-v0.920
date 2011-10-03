CREATE TABLE [Skills].[PersonTraining](
	[PersonId] [bigint] NOT NULL,
	[TrainingTypeId] [int] NOT NULL,
	[Info] [nvarchar](max) NOT NULL,
	[Institution] [nvarchar](max) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModificationDate] [datetime] NOT NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_PersonTraining] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[PersonId] ASC,
	[TrainingTypeId] ASC
)
)
