ALTER TABLE [dbo].[CFDivisionMapping]
	ADD CONSTRAINT [FK_CFDivisionMapping_DivisionId] FOREIGN KEY (DivisionId) REFERENCES [dbo].[PersonDivision] (DivisionId) ON DELETE NO ACTION ON UPDATE NO ACTION;


