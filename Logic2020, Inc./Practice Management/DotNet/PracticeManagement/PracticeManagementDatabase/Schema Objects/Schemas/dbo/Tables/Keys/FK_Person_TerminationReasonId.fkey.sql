ALTER TABLE [dbo].[Person]
	ADD CONSTRAINT [FK_Person_TerminationReasonId] 
	FOREIGN KEY (TerminationReasonId)
	REFERENCES TerminationReasons (TerminationReasonId) ON DELETE NO ACTION ON UPDATE NO ACTION;
