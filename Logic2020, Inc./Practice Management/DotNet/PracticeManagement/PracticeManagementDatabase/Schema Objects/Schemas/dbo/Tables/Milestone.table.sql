CREATE TABLE [dbo].[Milestone] (
    [ProjectId]             INT             NOT NULL,
    [MilestoneId]           INT             IDENTITY (1, 1) NOT NULL,
    [Description]           NVARCHAR (255)  NOT NULL,
    [Amount]                DECIMAL (18, 2) NULL,
    [StartDate]             DATETIME        NOT NULL,
    [ProjectedDeliveryDate] DATETIME        NOT NULL,
    [ActualDeliveryDate]    DATETIME        NULL,
    [IsHourlyAmount]        BIT             NOT NULL,
    [IsChargeable]          BIT             NOT NULL,
    [ConsultantsCanAdjust]  BIT             NOT NULL,
	[IsDefault]				BIT				NOT NULL DEFAULT(0)
);


