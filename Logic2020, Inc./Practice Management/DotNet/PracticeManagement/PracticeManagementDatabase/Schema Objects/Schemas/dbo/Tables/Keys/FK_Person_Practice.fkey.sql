ALTER TABLE [dbo].[Person]
    ADD CONSTRAINT [FK_Person_Practice] FOREIGN KEY ([DefaultPractice]) REFERENCES [dbo].[Practice] ([PracticeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


