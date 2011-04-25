CREATE TABLE [dbo].[Commission] (
    [ProjectId]        INT             NOT NULL,
    [PersonId]         INT             NOT NULL,
    [CommissionId]     INT             IDENTITY (1, 1) NOT NULL,
    [FractionOfMargin] DECIMAL (18, 2) NOT NULL,
    [CommissionType]   INT             NOT NULL,
    [ExpectedDatePaid] DATETIME        NULL,
    [ActualDatePaid]   DATETIME        NULL,
    [MarginTypeId]     INT             NULL
);


