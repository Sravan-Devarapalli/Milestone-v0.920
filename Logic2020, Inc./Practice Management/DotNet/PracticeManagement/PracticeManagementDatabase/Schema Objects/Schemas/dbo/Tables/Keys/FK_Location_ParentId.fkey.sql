ALTER TABLE [dbo].[Location]
	ADD CONSTRAINT [FK_Location_ParentId] FOREIGN KEY (ParentId) REFERENCES [dbo].[Location](LocationId) ON DELETE NO ACTION ON UPDATE NO ACTION;
