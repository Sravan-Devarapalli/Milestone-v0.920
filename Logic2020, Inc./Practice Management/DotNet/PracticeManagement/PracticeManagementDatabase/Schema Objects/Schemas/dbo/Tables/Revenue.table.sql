CREATE TABLE [dbo].[Revenue]
(
	[RevenueTypeId]   INT             IDENTITY (1, 1) NOT NULL,
	[RevenueName]     NVARCHAR (50)   NOT NULL,
	[IsDefault]	      BIT			  NOT NULL CONSTRAINT DF_Revenue_IsDefault DEFAULT 0,
	CONSTRAINT [PK_Revenue_RevenueTypeID]	PRIMARY KEY ([RevenueTypeId])
)

