ALTER TABLE [dbo].[PersonCalendarAuto]
    ADD CONSTRAINT [FK_PersonCalendarAuto_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [dbo].[Person] ([PersonId]) ON DELETE CASCADE ON UPDATE NO ACTION;


