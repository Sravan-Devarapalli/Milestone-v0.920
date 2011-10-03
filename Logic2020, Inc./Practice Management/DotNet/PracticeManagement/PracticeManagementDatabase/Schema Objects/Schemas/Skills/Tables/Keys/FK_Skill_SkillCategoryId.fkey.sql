ALTER TABLE [Skills].[Skill]  WITH CHECK ADD  CONSTRAINT [FK_Skill_SkillCategoryId] FOREIGN KEY([TenantId],[SkillCategoryId])
REFERENCES [Skills].[SkillCategory] ([TenantId],[SkillCategoryId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[Skill] CHECK CONSTRAINT [FK_Skill_SkillCategoryId]
