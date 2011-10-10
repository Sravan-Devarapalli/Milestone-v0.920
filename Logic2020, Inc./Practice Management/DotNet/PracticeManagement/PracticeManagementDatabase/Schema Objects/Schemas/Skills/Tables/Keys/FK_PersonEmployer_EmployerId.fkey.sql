ALTER TABLE [Skills].[PersonEmployer]  WITH CHECK ADD  CONSTRAINT [FK_PersonEmployer_EmployerId] FOREIGN KEY([EmployerId])
REFERENCES [Skills].[Employer] ([EmployerId])
ON UPDATE CASCADE
GO
ALTER TABLE [Skills].[PersonEmployer] CHECK CONSTRAINT [FK_PersonEmployer_EmployerId]
