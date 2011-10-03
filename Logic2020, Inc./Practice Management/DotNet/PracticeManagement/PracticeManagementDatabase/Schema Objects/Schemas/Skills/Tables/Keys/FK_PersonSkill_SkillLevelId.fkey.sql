ALTER TABLE [Skills].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonSkill_SkillLevelId] FOREIGN KEY([TenantId],[SkillLevelId])
REFERENCES [Skills].[SkillLevel] ([TenantId],[SkillLevelId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonSkill] CHECK CONSTRAINT [FK_PersonSkill_SkillLevelId]

