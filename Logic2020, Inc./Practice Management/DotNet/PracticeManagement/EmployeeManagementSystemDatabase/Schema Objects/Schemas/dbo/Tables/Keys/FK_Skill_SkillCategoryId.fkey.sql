ALTER TABLE [dbo].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_Skill_SkillCategoryId] FOREIGN KEY([SkillCategoryId])
REFERENCES [dbo].[SkillCategory] ([SkillCategoryId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[Skill] CHECK CONSTRAINT [FK_Skill_SkillCategoryId]
