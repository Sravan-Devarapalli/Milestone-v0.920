ALTER TABLE [Skills].[PersonEmployer]  WITH CHECK ADD  CONSTRAINT [FK_PersonEmployer_EmployerId] FOREIGN KEY([TenantId],[EmployerId])
REFERENCES [Skills].[Employer] ([TenantId],[EmployerId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonEmployer] CHECK CONSTRAINT [FK_PersonEmployer_EmployerId]
