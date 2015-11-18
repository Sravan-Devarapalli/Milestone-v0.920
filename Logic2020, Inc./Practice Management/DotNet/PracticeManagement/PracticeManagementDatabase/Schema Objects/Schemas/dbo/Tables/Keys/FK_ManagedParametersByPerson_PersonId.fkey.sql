ALTER TABLE [dbo].[ManagedParametersByPerson] 
ADD CONSTRAINT [FK_ManagedParametersByPerson_PersonId] FOREIGN KEY (PersonId) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
