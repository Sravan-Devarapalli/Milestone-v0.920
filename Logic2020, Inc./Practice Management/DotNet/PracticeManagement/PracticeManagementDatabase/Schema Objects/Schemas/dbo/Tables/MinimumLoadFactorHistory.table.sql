CREATE TABLE [dbo].[MinimumLoadFactorHistory](
	[OverheadFixedRateId] [int] NOT NULL,
	[TimescaleId] [int] NOT NULL,
	[Rate] [decimal](18, 5) NULL,
	[StartDate]	DATETIME NOT NULL,
	[EndDate]	DATETIME NULL,
 CONSTRAINT [PK_MinimumLoadFactorHistory] PRIMARY KEY CLUSTERED 
(
	[OverheadFixedRateId] ASC,
	[TimescaleId] ASC,
	[StartDate] ASC
)
)

