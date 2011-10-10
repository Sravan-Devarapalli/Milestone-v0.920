CREATE TABLE [Skills].[Skill](
	[SkillId] [int] NOT NULL IDENTITY(1,1),
	[SkillCategoryId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_Skill_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_Skill_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_Skill] PRIMARY KEY CLUSTERED 
(
	[SkillId] ASC
)
)
