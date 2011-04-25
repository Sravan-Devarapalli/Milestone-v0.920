CREATE TABLE [dbo].[ProjectBillingInfo] (
    [ProjectId]       INT            NOT NULL,
    [BillingContact]  NVARCHAR (100) NULL,
    [BillingPhone]    NVARCHAR (25)  NULL,
    [BillingEmail]    NVARCHAR (100) NULL,
    [BillingType]     NVARCHAR (25)  NULL,
    [BillingAddress1] NVARCHAR (100) NULL,
    [BillingAddress2] NVARCHAR (100) NULL,
    [BillingCity]     NVARCHAR (50)  NULL,
    [BillingState]    NVARCHAR (50)  NULL,
    [BillingZip]      NVARCHAR (10)  NULL,
    [PurchaseOrder]   NVARCHAR (25)  NULL
);


