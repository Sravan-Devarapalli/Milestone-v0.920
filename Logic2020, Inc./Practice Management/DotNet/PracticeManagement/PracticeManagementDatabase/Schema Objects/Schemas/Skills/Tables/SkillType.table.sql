CREATE TABLE [Skills].[SkillType](
	[SkillTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
	[IsActive]		[bit] NOT NULL CONSTRAINT DF_SkillType_IsActive DEFAULT(1),
	[IsDeleted]		[bit] NOT NULL CONSTRAINT DF_SkillType_IsDeleted DEFAULT(0),
 CONSTRAINT [PK_SkillType] PRIMARY KEY CLUSTERED 
(
	[SkillTypeId] ASC
)
)
