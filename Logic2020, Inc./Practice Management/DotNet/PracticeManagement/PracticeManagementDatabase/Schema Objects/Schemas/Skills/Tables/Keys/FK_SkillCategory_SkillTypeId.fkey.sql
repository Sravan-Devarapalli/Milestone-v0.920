ALTER TABLE [Skills].[SkillCategory]  WITH CHECK ADD  CONSTRAINT [FK_SkillCategory_SkillTypeId] FOREIGN KEY([SkillTypeId])
REFERENCES [Skills].[SkillType] ([SkillTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[SkillCategory] CHECK CONSTRAINT [FK_SkillCategory_SkillTypeId]
GO
