CREATE TABLE [dbo].[Vendor]
(
	 [Id]			    INT     IDENTITY (1, 1) NOT NULL,
	 [Name]				NVARCHAR (50)			NOT NULL,
	 [ContactName]		NVARCHAR (50)			NOT NULL,
	 [Status]			INT						NOT NULL,
	 [Email]			NVARCHAR (50)			NOT NULL,
	 [TelephoneNumber]	NVARCHAR (20)				NOT NULL,
	 [VendorTypeId]		INT						NOT NULL
	 CONSTRAINT [PK_Vendor_Id]	PRIMARY KEY ([Id])
)

