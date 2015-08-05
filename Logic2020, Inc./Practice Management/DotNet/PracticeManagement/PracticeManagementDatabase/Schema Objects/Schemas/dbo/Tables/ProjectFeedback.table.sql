CREATE TABLE [dbo].[ProjectFeedback]
(
	[FeedbackId]	INT	IDENTITY (1, 1) NOT NULL,
	[ProjectId]		INT	NOT NULL,
	[PersonId]		INT NOT NULL,
	[ReviewPeriodStartDate]	DATETIME NOT NULL,
	[ReviewPeriodEndDate]	DATETIME NOT NULL,
	[DueDate]				DATETIME NOT NULL,
	[FeedbackStatusId]	INT NOT NULL,
	[IsCanceled]	BIT NOT NULL,
	[CompletionCertificateBy] INT,
	[CompletionCertificateDate] DATETIME,
	[CancelationReason] NVARCHAR(MAX),
	[NextIntialMailSendDate] DATETIME,
	[IsGap]			BIT NULL
)

