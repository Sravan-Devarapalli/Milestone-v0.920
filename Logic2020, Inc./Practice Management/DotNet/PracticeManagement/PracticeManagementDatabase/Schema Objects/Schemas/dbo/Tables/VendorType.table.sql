CREATE TABLE [dbo].[VendorType]
(
	[Id]      INT         IDENTITY (1, 1) NOT NULL,
	[Name]    NVARCHAR (50)   NOT NULL
	CONSTRAINT [PK_VendorType_Id]	PRIMARY KEY ([Id])
)

