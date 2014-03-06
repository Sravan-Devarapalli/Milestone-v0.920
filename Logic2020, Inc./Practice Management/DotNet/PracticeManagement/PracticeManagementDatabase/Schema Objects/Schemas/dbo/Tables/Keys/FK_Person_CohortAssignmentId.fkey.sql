ALTER TABLE [dbo].[Person]
	ADD CONSTRAINT [FK_Person_CohortAssignmentId] 
	FOREIGN KEY (CohortAssignmentId)
	REFERENCES [CohortAssignment] ([CohortAssignmentId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


