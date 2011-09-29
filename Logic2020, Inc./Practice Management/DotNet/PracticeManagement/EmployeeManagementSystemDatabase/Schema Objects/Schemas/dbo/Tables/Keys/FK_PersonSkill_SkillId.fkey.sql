ALTER TABLE [dbo].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonSkill_SkillId] FOREIGN KEY([SkillId])
REFERENCES [dbo].[Skill] ([SkillId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonSkill] CHECK CONSTRAINT [FK_PersonSkill_SkillId]
