ALTER TABLE [dbo].[Person]
	ADD CONSTRAINT [FK_Person_LocationId] FOREIGN KEY (LocationId)	REFERENCES [dbo].[Location](LocationId) ON DELETE NO ACTION ON UPDATE NO ACTION;


