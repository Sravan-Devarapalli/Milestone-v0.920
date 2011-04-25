ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [FK_Project_Practice1] FOREIGN KEY ([PracticeId]) REFERENCES [dbo].[Practice] ([PracticeId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


