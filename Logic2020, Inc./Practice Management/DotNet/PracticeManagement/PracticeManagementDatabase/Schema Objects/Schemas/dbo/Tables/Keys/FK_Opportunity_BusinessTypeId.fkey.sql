	ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_BusinessTypeId] FOREIGN KEY ([BusinessTypeId]) REFERENCES [dbo].[BusinessType] ([BusinessTypeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
