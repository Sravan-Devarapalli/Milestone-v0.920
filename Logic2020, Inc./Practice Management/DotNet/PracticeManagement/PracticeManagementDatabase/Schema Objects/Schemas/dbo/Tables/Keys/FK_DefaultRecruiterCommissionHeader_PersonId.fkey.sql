ALTER TABLE [dbo].[DefaultRecruiterCommissionHeader]
ADD CONSTRAINT [FK_DefaultRecruiterCommissionHeader_PersonId] 
FOREIGN KEY(PersonId) 
REFERENCES  dbo.Person(PersonId)

