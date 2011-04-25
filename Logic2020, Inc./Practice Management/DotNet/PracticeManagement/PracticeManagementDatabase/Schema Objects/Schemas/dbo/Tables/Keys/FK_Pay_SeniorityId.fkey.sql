ALTER TABLE [dbo].[Pay]
    ADD CONSTRAINT [FK_Pay_SeniorityId] FOREIGN KEY ([SeniorityId]) 
	REFERENCES [dbo].[Seniority] ([SeniorityId]) ON DELETE NO ACTION ON UPDATE NO ACTION;

