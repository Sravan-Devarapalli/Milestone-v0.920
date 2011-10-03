CREATE TABLE [Skills].[SkillCategory](
	[SkillCategoryId] [INT] NOT NULL IDENTITY(1,1),
	[SkillTypeId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_SkillCategory] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[SkillCategoryId] ASC
)
)
 
