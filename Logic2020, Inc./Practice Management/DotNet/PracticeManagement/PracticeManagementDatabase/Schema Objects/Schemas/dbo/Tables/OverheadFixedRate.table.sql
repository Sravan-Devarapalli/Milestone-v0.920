CREATE TABLE [dbo].[OverheadFixedRate] (
    [OverheadFixedRateId] INT             IDENTITY (1, 1) NOT NULL,
    [Description]         NVARCHAR (255)  NOT NULL,
    [Rate]                DECIMAL (18, 5) NOT NULL,
    [StartDate]           DATETIME        NOT NULL,
    [EndDate]             DATETIME        NULL,
    [RateType]            INT             NOT NULL,
    [Inactive]            BIT             NOT NULL,
    [IsCogs]              BIT             NOT NULL,
	[IsMinimumLoadFactor] BIT NOT NULL	CONSTRAINT DF_OverheadFixedRate_IsMinimumLoadFactor DEFAULT 0
);


