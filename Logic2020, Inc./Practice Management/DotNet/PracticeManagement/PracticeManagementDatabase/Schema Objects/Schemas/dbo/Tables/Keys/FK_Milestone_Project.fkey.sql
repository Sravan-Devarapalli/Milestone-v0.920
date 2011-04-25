ALTER TABLE [dbo].[Milestone]
    ADD CONSTRAINT [FK_Milestone_Project] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


