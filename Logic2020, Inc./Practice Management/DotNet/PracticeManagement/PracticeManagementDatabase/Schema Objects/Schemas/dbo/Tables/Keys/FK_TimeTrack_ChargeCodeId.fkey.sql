﻿ALTER TABLE [dbo].[TimeTrack]
	ADD CONSTRAINT [FK_TimeTrack_ChargeCodeId] 
	FOREIGN KEY (ChargeCodeId) 
	REFERENCES dbo.ChargeCode(Id) ON DELETE NO ACTION ON UPDATE NO ACTION;
