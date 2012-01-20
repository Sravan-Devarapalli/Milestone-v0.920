ALTER TABLE [dbo].[ChargeCodeTurnOffHistory]
	ADD CONSTRAINT [FK_ChargeCodeTurnOffHistory_ChargeCodeId] 
	FOREIGN KEY (ChargeCodeId) 
	REFERENCES dbo.ChargeCode (Id) ON DELETE NO ACTION ON UPDATE NO ACTION;
