ALTER TABLE [dbo].[Attribution]
	ADD CONSTRAINT [FK_Attribution_AttributionRecordId] 
	FOREIGN KEY (AttributionRecordTypeId)
	REFERENCES dbo.AttributionRecordTypes (AttributionRecordId)	 ON DELETE NO ACTION ON UPDATE NO ACTION;


