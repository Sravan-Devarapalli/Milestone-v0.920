ALTER TABLE [Skills].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonSkill_SkillId] FOREIGN KEY([TenantId],[SkillId])
REFERENCES [Skills].[Skill] ([TenantId],[SkillId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonSkill] CHECK CONSTRAINT [FK_PersonSkill_SkillId]
