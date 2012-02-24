ALTER TABLE dbo.PersonCalendar
ADD CONSTRAINT [FK_PersonCalendar_SubstituteDate] FOREIGN KEY (SubstituteDate) 
REFERENCES [dbo].[Calendar] ([Date]) ON DELETE NO ACTION ON UPDATE NO ACTION;
