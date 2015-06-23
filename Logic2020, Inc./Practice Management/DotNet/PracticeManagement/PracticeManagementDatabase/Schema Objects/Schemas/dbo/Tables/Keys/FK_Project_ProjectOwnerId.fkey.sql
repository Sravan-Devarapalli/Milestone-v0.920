---------------------------
-- Created By: ThulasiRam.P
-- Create Date: 2012-05-21
---------------------------
ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [FK_Project_ProjectOwnerId] FOREIGN KEY ([ProjectManagerId]) 
	REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
