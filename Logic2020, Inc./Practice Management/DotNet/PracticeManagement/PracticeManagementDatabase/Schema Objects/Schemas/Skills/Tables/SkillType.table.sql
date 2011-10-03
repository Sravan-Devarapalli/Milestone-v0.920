CREATE TABLE [Skills].[SkillType](
	[SkillTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_SkillType] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[SkillTypeId] ASC
)
)
