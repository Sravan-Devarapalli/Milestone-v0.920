ALTER TABLE [dbo].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonSkill_SkillLevelId] FOREIGN KEY([SkillLevelId])
REFERENCES [dbo].[SkillLevel] ([SkillLevelId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonSkill] CHECK CONSTRAINT [FK_PersonSkill_SkillLevelId]

