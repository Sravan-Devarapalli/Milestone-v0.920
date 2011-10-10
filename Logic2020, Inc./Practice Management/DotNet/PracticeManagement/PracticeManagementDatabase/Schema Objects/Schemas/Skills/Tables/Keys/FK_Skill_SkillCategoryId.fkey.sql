ALTER TABLE [Skills].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_Skill_SkillCategoryId] FOREIGN KEY([SkillCategoryId])
REFERENCES [Skills].[SkillCategory] ([SkillCategoryId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[Skill] CHECK CONSTRAINT [FK_Skill_SkillCategoryId]
