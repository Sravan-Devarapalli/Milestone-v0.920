ALTER TABLE [dbo].[ProjectBillingInfo]
    ADD CONSTRAINT [FK_ProjectBillingInfo_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


