ALTER TABLE [dbo].[Person]
	ADD CONSTRAINT [FK_Person_JobSeekerStatusId] 
	FOREIGN KEY (JobSeekerStatusId)
	REFERENCES [JobSeekerStatus] ([JobSeekerStatusId]) ON DELETE NO ACTION ON UPDATE NO ACTION;


