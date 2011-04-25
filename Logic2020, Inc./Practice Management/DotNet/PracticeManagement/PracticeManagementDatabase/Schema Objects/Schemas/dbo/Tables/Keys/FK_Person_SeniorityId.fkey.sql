ALTER TABLE [dbo].[Person]
    ADD CONSTRAINT [FK_Person_SeniorityId] FOREIGN KEY ([SeniorityId]) REFERENCES [dbo].[Seniority] ([SeniorityId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


