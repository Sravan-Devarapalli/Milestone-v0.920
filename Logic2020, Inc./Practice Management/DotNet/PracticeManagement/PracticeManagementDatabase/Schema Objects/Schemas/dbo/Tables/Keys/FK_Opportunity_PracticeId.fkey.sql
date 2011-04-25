ALTER TABLE [dbo].[Opportunity]
    ADD CONSTRAINT [FK_Opportunity_PracticeId] FOREIGN KEY ([PracticeId]) REFERENCES [dbo].[Practice] ([PracticeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


