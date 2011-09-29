CREATE TABLE [dbo].[SkillCategory](
	[SkillCategoryId] [INT] NOT NULL IDENTITY(1,1),
	[SkillTypeId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_SkillCategory] PRIMARY KEY CLUSTERED 
(
	[SkillCategoryId] ASC
)
)
 
