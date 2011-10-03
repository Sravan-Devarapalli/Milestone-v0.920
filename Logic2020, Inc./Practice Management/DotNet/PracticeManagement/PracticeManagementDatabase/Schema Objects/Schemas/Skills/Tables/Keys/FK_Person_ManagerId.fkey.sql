ALTER TABLE [Skills].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_ManagerId] FOREIGN KEY([TenantId],[ManagerId])
REFERENCES [Skills].[Person] ([TenantId],[PersonId])
GO
ALTER TABLE [Skills].[Person] CHECK CONSTRAINT [FK_Person_ManagerId]
GO
