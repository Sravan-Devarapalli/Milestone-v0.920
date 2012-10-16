ALTER TABLE [dbo].[PracticeCapabilities]
ADD CONSTRAINT [FK_PracticeCapabilities_PracticeId] FOREIGN KEY ([PracticeId]) REFERENCES [dbo].[Practice]([PracticeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
