CREATE TABLE [dbo].[TrainingType](
	[TrainingTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_TrainingType] PRIMARY KEY CLUSTERED 
(
	[TrainingTypeId] ASC
)
)
