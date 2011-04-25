CREATE TABLE dbo.StringResources (
    Id                    INT IDENTITY(1,1) NOT NULL,
    TypeId				  INT NOT NULL,
    ResourcesKey          NVARCHAR(255) NOT NULL,
    Value                 NVARCHAR(MAX) NOT NULL,
   CONSTRAINT PK_StringResources_ResourcesKey PRIMARY KEY CLUSTERED(ResourcesKey),
   CONSTRAINT FK_StringResources_TypeId FOREIGN KEY(TypeId) REFERENCES  dbo.ResourceType(TypeId)
);


