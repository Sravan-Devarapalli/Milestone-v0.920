CREATE TABLE [Skills].[SkillLevel](
	[SkillLevelId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_SkillLevel] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[SkillLevelId] ASC
)
)
