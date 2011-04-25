CREATE TABLE dbo.OpportunityPriorities (
    Id                   INT IDENTITY(1,1) NOT NULL,
    Priority			 NVARCHAR(1) NOT NULL,
    Description          NVARCHAR(MAX) NOT NULL
   CONSTRAINT PK_OpportunityPriorities_Priority PRIMARY KEY CLUSTERED(Priority)
);
