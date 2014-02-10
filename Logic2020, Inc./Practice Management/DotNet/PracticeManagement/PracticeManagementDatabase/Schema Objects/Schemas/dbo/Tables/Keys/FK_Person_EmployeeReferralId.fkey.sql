ALTER TABLE [dbo].[Person]
	ADD CONSTRAINT [FK_Person_EmployeeReferralId] 
	FOREIGN KEY (EmployeeReferralId)
	REFERENCES Person (PersonId) ON DELETE NO ACTION ON UPDATE NO ACTION;


