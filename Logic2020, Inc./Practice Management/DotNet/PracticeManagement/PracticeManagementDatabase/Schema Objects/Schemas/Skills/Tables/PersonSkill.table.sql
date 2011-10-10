CREATE TABLE [Skills].[PersonSkill](
	[SkillId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[YearsExperience] [int] NULL,
	[SkillLevelId] [int] NOT NULL,
	[LastUsed] [int] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonSkill] PRIMARY KEY CLUSTERED 
(
	[SkillId] ASC,
	[PersonId] ASC
)
)
