ALTER TABLE [dbo].[Person]  WITH CHECK ADD  CONSTRAINT [FK_Person_ManagerId] FOREIGN KEY([ManagerId])
REFERENCES [dbo].[Person] ([PersonId])
GO
ALTER TABLE [dbo].[Person] CHECK CONSTRAINT [FK_Person_ManagerId]
GO
