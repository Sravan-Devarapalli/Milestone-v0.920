ALTER TABLE [dbo].[Commission]
    ADD CONSTRAINT [FK_Commission_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


