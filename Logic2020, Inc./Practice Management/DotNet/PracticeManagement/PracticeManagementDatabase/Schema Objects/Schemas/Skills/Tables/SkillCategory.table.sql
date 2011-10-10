CREATE TABLE [Skills].[SkillCategory](
	[SkillCategoryId] [INT] NOT NULL IDENTITY(1,1),
	[SkillTypeId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_SkillCategory_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_SkillCategory_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_SkillCategory] PRIMARY KEY CLUSTERED 
(
	[SkillCategoryId] ASC
)
)
 
