ALTER TABLE [dbo].[OpportunityPersons]
ADD CONSTRAINT [FK_OpportunityPersons_PersonId] 
FOREIGN KEY(PersonId) 
REFERENCES  dbo.Person(PersonId)
