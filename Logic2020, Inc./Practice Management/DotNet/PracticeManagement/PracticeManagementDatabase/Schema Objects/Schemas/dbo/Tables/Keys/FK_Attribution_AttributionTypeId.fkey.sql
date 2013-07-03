ALTER TABLE [dbo].[Attribution]
	ADD CONSTRAINT [FK_Attribution_AttributionTypeId] 
	FOREIGN KEY ([AttributionTypeId])
	REFERENCES dbo.AttributionTypes (AttributionTypeId)	 ON DELETE NO ACTION ON UPDATE NO ACTION;


