ALTER TABLE [dbo].[CFDivisionMapping]
	ADD CONSTRAINT [FK_CFDivisionMapping_CFDivisionId] FOREIGN KEY (CFDivisionId) REFERENCES [dbo].[Division_CF](DivisionId) ON DELETE NO ACTION ON UPDATE NO ACTION;


