CREATE TABLE [Skills].[PersonContactInfo](
	[ContactInfoTypeId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[Info] [nvarchar](max) NULL,
	[ModificationDate] [datetime] NOT NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_PersonContactInfo] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[ContactInfoTypeId] ASC,
	[PersonId] ASC
)
)
