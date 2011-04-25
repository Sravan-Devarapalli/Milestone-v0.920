ALTER TABLE [dbo].[PersonCalendar]
    ADD CONSTRAINT [FK_PersonCalendar_Date] FOREIGN KEY ([Date]) REFERENCES [dbo].[Calendar] ([Date]) ON DELETE NO ACTION ON UPDATE NO ACTION;


