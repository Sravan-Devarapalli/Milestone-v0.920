CREATE TABLE [dbo].[PricingList]
(
	[PricingListId]  INT IDENTITY(0,1)             NOT NULL, 
    [ClientId]       INT                           NOT NULL,
    [Name]           NVARCHAR(200)                 NOT NULL,
	[IsDefault]      BIT                           NOT NULL CONSTRAINT DF_PricingList_IsDefault DEFAULT(0),
	 PRIMARY KEY CLUSTERED ([PricingListId] ASC) WITH (IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF)
)

