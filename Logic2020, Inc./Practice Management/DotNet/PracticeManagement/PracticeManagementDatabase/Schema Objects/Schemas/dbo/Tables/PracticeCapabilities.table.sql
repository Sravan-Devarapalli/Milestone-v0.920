CREATE TABLE [dbo].[PracticeCapabilities]
(
	[CapabilityId]		INT	IDENTITY (1, 1) NOT NULL,
	[PracticeId]		INT					NOT NULL,
    [CapabilityName]	VARCHAR (100)		NOT NULL,
	CONSTRAINT PK_PracticeCapabilities_CapabilityId PRIMARY KEY CLUSTERED([CapabilityId]),
	CONSTRAINT [UQ_PracticeCapabilities_CapabilityName] UNIQUE NONCLUSTERED([CapabilityName]) ON [PRIMARY]
)

