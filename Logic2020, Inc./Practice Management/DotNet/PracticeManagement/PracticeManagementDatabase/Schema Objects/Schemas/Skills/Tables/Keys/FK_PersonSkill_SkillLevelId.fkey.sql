ALTER TABLE [Skills].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonSkill_SkillLevelId] FOREIGN KEY([SkillLevelId])
REFERENCES [Skills].[SkillLevel] ([SkillLevelId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonSkill] CHECK CONSTRAINT [FK_PersonSkill_SkillLevelId]

