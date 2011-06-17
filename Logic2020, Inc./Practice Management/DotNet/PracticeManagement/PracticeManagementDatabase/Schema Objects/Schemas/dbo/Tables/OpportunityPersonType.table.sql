CREATE TABLE dbo.OpportunityPersonType
(
	[Id]   INT IDENTITY(1,1) NOT NULL,
	[PersonType] NVARCHAR(255) NOT NULL,
	CONSTRAINT PK_OpportunityPersonType_Id PRIMARY KEY CLUSTERED([Id])
); 
