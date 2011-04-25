ALTER TABLE [dbo].[PersonCalendar]
    ADD CONSTRAINT [FK_PersonCalendar_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


