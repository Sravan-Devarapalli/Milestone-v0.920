ALTER TABLE [dbo].[ProjectFeedback]
	ADD CONSTRAINT [FK_Feedback_StatusId] FOREIGN KEY ([FeedbackStatusId]) REFERENCES [dbo].[ProjectFeedbackStatus] ([FeedbackStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


