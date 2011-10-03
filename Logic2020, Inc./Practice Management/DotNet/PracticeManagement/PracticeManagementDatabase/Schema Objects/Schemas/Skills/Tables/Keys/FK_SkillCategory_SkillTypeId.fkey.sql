ALTER TABLE [Skills].[SkillCategory]  WITH CHECK ADD  CONSTRAINT [FK_SkillCategory_SkillTypeId] FOREIGN KEY([TenantId],[SkillTypeId])
REFERENCES [Skills].[SkillType] ([TenantId],[SkillTypeId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[SkillCategory] CHECK CONSTRAINT [FK_SkillCategory_SkillTypeId]
GO
