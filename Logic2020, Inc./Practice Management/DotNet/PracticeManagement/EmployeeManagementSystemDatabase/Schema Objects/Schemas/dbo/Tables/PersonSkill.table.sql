CREATE TABLE [dbo].[PersonSkill](
	[SkillId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[YearsExperience] [int] NULL,
	[SkillLevelId] [int] NOT NULL,
	[LastUsed] [int] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonSkill] PRIMARY KEY CLUSTERED 
(
	[SkillId] ASC,
	[PersonId] ASC
)
)
