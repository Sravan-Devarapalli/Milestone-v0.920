ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_ClientId] FOREIGN KEY ([ClientId]) REFERENCES [dbo].[Client] ([ClientId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


