CREATE TABLE [dbo].[Offering]
(
	[OfferingId]    INT             IDENTITY (1, 1) NOT NULL,
	[Name]          NVARCHAR (50)   NOT NULL,
	CONSTRAINT [PK_Offering_OfferingId]	PRIMARY KEY ([OfferingId])
)

