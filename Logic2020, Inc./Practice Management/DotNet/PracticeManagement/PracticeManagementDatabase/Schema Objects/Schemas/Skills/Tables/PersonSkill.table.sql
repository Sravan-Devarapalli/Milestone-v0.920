CREATE TABLE [Skills].[PersonSkill](
	[SkillId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[YearsExperience] [int] NULL,
	[SkillLevelId] [int] NOT NULL,
	[LastUsed] [int] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModificationDate] [datetime] NOT NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_PersonSkill] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[SkillId] ASC,
	[PersonId] ASC
)
)
