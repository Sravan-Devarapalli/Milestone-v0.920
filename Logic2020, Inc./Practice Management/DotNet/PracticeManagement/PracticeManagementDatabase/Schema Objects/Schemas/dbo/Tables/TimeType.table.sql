﻿CREATE TABLE [dbo].[TimeType] (
    [TimeTypeId]		INT	IDENTITY (1, 1) NOT NULL,
    [Code]				NVARCHAR(5)			NOT NULL,
    [Name]				VARCHAR (50)		NOT NULL,
    [IsDefault]			BIT					NOT NULL,
    [IsAllowedToEdit]	BIT					NULL,
    [IsInternal]		BIT					NOT NULL,
	[IsActive]			BIT					NOT NULL
);

