ALTER TABLE [Skills].[PersonSkill]  WITH CHECK ADD  CONSTRAINT [FK_PersonSkill_PersonId] FOREIGN KEY([TenantId],[PersonId])
REFERENCES [Skills].[Person] ([TenantId],[PersonId])
GO
ALTER TABLE [Skills].[PersonSkill] CHECK CONSTRAINT [FK_PersonSkill_PersonId]
