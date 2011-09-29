CREATE TABLE [dbo].[Practice](
	[PracticeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_Practice] PRIMARY KEY CLUSTERED 
(
	[PracticeId] ASC
)
)
