CREATE TABLE [dbo].[OpportunityPersons]
(
	Id              INT IDENTITY(1,1) NOT NULL,
	OpportunityId	INT NOT NULL, 
	PersonId		INT NOT NULL,
	OpportunityPersonTypeId INT NOT NULL
	CONSTRAINT PK_OpportunityPersons_Id PRIMARY KEY CLUSTERED(Id),
    CONSTRAINT FK_OpportunityPersons_OpportunityId FOREIGN KEY(OpportunityId) REFERENCES  dbo.Opportunity(OpportunityId),
	CONSTRAINT FK_OpportunityPersons_OpportunityPersonTypeId FOREIGN KEY(OpportunityPersonTypeId) REFERENCES  dbo.OpportunityPersonType([Id])
); 
