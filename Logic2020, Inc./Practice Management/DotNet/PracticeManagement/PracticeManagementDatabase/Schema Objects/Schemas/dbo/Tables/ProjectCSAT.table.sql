CREATE TABLE [dbo].[ProjectCSAT]
(
	[CSATId]			INT		IDENTITY (1, 1) NOT NULL,
	[ProjectId]			INT						NOT NULL,
	ReviewStartDate		DATETIME				NOT NULL,
	ReviewEndDate		DATETIME				NOT NULL,
	CompletionDate		DATETIME				NOT NULL,
	ReviewerId			INT						NOT NULL,
	ReferralScore		INT						NOT NULL,
	Comments			NVARCHAR (MAX)				NULL,
	CreatedDate			DATETIME				NOT NULL,
	ModifiedDate		DATETIME				NOT NULL,
	ModifiedBy			INT             		NOT NULL
	CONSTRAINT PK_ProjectCSAT_CSATId PRIMARY KEY CLUSTERED([CSATId])
)

