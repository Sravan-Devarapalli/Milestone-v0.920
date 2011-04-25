ALTER TABLE [dbo].[Practice]
    ADD CONSTRAINT [FK_Practice_Person] FOREIGN KEY ([PracticeManagerId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


