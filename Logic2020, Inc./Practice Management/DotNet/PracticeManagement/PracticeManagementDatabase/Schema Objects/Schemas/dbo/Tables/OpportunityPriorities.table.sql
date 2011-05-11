CREATE TABLE dbo.OpportunityPriorities (
    Id                   INT IDENTITY(1,1) NOT NULL,
    Priority			 NVARCHAR(255) NOT NULL,
    Description          NVARCHAR(MAX) NOT NULL,
	sortOrder			 INT
   CONSTRAINT PK_OpportunityPriorities_Id PRIMARY KEY CLUSTERED(Id)
);
