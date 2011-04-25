ALTER TABLE [dbo].[BilledTime]
    ADD CONSTRAINT [FK_BilledTime_Milestone] FOREIGN KEY ([MilestoneId]) REFERENCES [dbo].[Milestone] ([MilestoneId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


