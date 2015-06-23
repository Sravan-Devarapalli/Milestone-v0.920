ALTER TABLE [dbo].[Project]
ADD CONSTRAINT [FK_Project_SeniorManagerId] FOREIGN KEY (EngagementManagerId) 
REFERENCES [dbo].[Person] ([PersonId]) ON DELETE NO ACTION ON UPDATE NO ACTION;
