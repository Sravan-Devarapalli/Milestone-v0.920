CREATE TABLE [Skills].[PersonTraining](
	[PersonId] [int] NOT NULL,
	[TrainingTypeId] [int] NOT NULL,
	[Info] [nvarchar](max) NOT NULL,
	[Institution] [nvarchar](max) NOT NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonTraining] PRIMARY KEY CLUSTERED 
(
	[PersonId] ASC,
	[TrainingTypeId] ASC
)
)
