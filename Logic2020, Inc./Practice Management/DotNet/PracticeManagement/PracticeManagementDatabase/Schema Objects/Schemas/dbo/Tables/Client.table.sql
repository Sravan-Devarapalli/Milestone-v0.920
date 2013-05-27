CREATE TABLE [dbo].[Client] (
	[ClientId]					INT	IDENTITY (1, 1) NOT NULL,
	[Code]						NVARCHAR (5)	NOT NULL,
	[DefaultDiscount]			DECIMAL (18, 2)	NOT NULL,
	[DefaultTerms]				INT				NOT NULL,
	[DefaultSalespersonID]		INT				NOT NULL,
	[Name]						NVARCHAR (100)	NOT NULL,
	[Inactive]					BIT				NOT NULL,
	[IsChargeable]				BIT				NOT NULL,
	[DefaultDirectorID]			INT,
	[IsMarginColorInfoEnabled]	BIT				NULL,
	[IsInternal]				BIT				NOT NULL,
	[IsNoteRequired]            BIT				NOT NULL,
	[IsHouseAccount]             BIT			NOT NULL CONSTRAINT DF_Client_IsHouseAccount DEFAULT 0
);

