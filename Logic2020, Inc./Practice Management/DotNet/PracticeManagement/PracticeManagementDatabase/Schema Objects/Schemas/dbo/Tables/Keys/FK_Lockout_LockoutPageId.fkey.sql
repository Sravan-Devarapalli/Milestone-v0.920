ALTER TABLE [dbo].[Lockout]
	ADD CONSTRAINT [FK_Lockout_LockoutPageId] 
	FOREIGN KEY (LockoutPageId)
	REFERENCES LockoutPages (LockoutPageId)	ON DELETE NO ACTION ON UPDATE NO ACTION;
