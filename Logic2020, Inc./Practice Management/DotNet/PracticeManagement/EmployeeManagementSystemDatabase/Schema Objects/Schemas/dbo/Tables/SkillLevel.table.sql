CREATE TABLE [dbo].[SkillLevel](
	[SkillLevelId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_SkillLevel] PRIMARY KEY CLUSTERED 
(
	[SkillLevelId] ASC
)
)
