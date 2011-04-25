CREATE TABLE dbo.ResourceType
(
	[TypeId]   INT IDENTITY(1,1) NOT NULL,
	[Description] NVARCHAR(255) NOT NULL,
	CONSTRAINT PK_ResourceTypes_TypeId PRIMARY KEY CLUSTERED([TypeId])
);
