﻿ALTER TABLE [dbo].[ChargeCode]
	ADD CONSTRAINT [FK_ChargeCode_TimeTypeId] 
	FOREIGN KEY (TimeTypeId) 
	REFERENCES dbo.TimeType (TimeTypeId) ON DELETE NO ACTION ON UPDATE NO ACTION;
