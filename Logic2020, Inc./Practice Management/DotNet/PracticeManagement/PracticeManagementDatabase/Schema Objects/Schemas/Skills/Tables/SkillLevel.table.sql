CREATE TABLE [Skills].[SkillLevel](
	[SkillLevelId]	[int] NOT NULL IDENTITY(1,1),
	[Description]	[nvarchar](max) NULL,
	[Definition]	[nvarchar](max) NULL,
	[DisplayOrder]	[tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_SkillLevel_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_SkillLevel_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_SkillLevel] PRIMARY KEY CLUSTERED 
(
	[SkillLevelId] ASC
)
)
