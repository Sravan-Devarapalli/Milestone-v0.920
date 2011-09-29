CREATE TABLE [dbo].[SkillType](
	[SkillTypeId] [int] NOT NULL IDENTITY(1,1),
	[Description] [nvarchar](max) NULL,
	[DisplayOrder] [tinyint] NULL,
 CONSTRAINT [PK_SkillType] PRIMARY KEY CLUSTERED 
(
	[SkillTypeId] ASC
)
)
