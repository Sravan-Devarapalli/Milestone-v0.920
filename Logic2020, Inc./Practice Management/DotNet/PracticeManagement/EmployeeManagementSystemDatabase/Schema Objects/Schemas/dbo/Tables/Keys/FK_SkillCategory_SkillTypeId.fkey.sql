ALTER TABLE [dbo].[SkillCategory]  WITH CHECK ADD  CONSTRAINT [FK_SkillCategory_SkillTypeId] FOREIGN KEY([SkillTypeId])
REFERENCES [dbo].[SkillType] ([SkillTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[SkillCategory] CHECK CONSTRAINT [FK_SkillCategory_SkillTypeId]
GO
