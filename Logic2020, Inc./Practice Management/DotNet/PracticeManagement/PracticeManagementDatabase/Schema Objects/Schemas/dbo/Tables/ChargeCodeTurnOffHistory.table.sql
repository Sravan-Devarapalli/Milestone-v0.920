CREATE TABLE [dbo].[ChargeCodeTurnOffHistory]
(
    ChargeCodeId        INT        NOT NULL,
    StartDate           DATETIME   NOT NULL,
    EndDate             DATETIME   NULL,
	CONSTRAINT PK_ChargeCodeTurnOffHistory_ChargeCodeIdStartDate PRIMARY KEY CLUSTERED (ChargeCodeId ASC, StartDate ASC)
);

