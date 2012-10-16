CREATE TABLE [dbo].[ProjectCapabilities]
(
	[Id]				INT	IDENTITY (1, 1) NOT NULL,
    [ProjectId]			INT					NOT NULL,
    [CapabilityId]		INT					NOT NULL,
	CONSTRAINT PK_ProjectCapabilities_Id PRIMARY KEY CLUSTERED([Id]),
	CONSTRAINT [UQ_ProjectCapabilities_ProjectId_CapabilityId] UNIQUE NONCLUSTERED([ProjectId] ASC, [CapabilityId] ASC) ON [PRIMARY]
)

