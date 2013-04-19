ALTER TABLE [dbo].[PricingList]
	ADD CONSTRAINT [FK_PricingList_ClientId] 
	FOREIGN KEY ([ClientId])
	REFERENCES [dbo].[Client] ([ClientId])	ON DELETE NO ACTION ON UPDATE NO ACTION


