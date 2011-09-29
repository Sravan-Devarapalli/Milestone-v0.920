CREATE TABLE [dbo].[Skill](
	[SkillId] [int] NOT NULL IDENTITY(1,1),
	[SkillCategoryId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_Skill] PRIMARY KEY CLUSTERED 
(
	[SkillId] ASC
)
)
