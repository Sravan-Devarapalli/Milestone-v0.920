ALTER TABLE [dbo].[PersonEmployer]  WITH CHECK ADD  CONSTRAINT [FK_PersonEmployer_EmployerId] FOREIGN KEY([EmployerId])
REFERENCES [dbo].[Employer] ([EmployerId])
ON UPDATE CASCADE
GO
ALTER TABLE [dbo].[PersonEmployer] CHECK CONSTRAINT [FK_PersonEmployer_EmployerId]
