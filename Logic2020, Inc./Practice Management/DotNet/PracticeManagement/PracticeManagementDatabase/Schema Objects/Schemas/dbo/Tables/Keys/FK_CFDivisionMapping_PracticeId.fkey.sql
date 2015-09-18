ALTER TABLE [dbo].[CFDivisionMapping]
	ADD CONSTRAINT [FK_CFDivisionMapping_PracticeId] FOREIGN KEY (PracticeId) REFERENCES  [dbo].[Practice](PracticeId) ON DELETE NO ACTION ON UPDATE NO ACTION;


