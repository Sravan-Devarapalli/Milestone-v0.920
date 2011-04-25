CREATE TABLE [dbo].[OverheadRateType] (
    [OverheadRateTypeId] INT           NOT NULL,
    [Name]               NVARCHAR (50) NOT NULL,
    [IsPercentage]       BIT           NOT NULL,
    [HoursToCollect]     INT           NOT NULL
);


