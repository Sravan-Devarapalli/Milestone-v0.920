ALTER TABLE [dbo].[ProjectFeedback]
	ADD CONSTRAINT [FK_Feedback_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Project] ([ProjectId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


