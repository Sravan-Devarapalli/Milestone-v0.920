ALTER TABLE [dbo].[PersonCalendarAuto]
    ADD CONSTRAINT [FK_PersonCalendarAuto_Date] FOREIGN KEY ([Date]) REFERENCES [dbo].[Calendar] ([Date]) ON DELETE CASCADE ON UPDATE NO ACTION;


