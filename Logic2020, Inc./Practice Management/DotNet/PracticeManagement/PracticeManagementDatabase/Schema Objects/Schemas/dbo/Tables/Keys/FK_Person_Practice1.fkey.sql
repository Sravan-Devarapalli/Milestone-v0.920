ALTER TABLE [dbo].[Person]
    ADD CONSTRAINT [FK_Person_Practice1] FOREIGN KEY ([PracticeOwnedId]) REFERENCES [dbo].[Practice] ([PracticeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


