CREATE TABLE [Skills].[Skill](
	[SkillId] [int] NOT NULL IDENTITY(1,1),
	[SkillCategoryId] [int] NOT NULL,
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_Skill] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[SkillId] ASC
)
)
