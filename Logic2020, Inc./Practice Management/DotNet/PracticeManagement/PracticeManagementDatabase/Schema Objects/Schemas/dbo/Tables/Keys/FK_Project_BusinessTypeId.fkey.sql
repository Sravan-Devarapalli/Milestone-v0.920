ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [FK_Project_BusinessTypeId] FOREIGN KEY ([BusinessTypeId]) REFERENCES [dbo].[BusinessType] ([BusinessTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
