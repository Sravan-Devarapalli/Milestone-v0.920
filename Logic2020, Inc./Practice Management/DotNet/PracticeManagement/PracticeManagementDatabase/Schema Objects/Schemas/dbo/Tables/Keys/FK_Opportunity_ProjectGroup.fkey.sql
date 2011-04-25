ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_ProjectGroup] FOREIGN KEY ([GroupId]) REFERENCES [dbo].[ProjectGroup] ([GroupId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


