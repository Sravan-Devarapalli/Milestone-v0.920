ALTER TABLE [dbo].[ProjectFeedback]
	ADD CONSTRAINT [FK_Feedback_MilestonePersonId] FOREIGN KEY ([MilestonePersonId]) REFERENCES [dbo].[MilestonePerson] ([MilestonePersonId])	ON DELETE NO ACTION ON UPDATE NO ACTION;


