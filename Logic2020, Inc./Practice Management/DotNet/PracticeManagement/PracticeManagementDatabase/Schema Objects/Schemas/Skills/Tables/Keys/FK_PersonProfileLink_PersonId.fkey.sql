ALTER TABLE [Skills].[PersonProfile]
	ADD CONSTRAINT [FK_PersonProfile_PersonId] 
	FOREIGN KEY (PersonId)
	REFERENCES  [dbo].[Person] (PersonId)	
    ON DELETE NO ACTION ON UPDATE NO ACTION;

