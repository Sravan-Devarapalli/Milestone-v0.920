CREATE TABLE [Skills].[PersonContactInfo](
	[ContactInfoTypeId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[Info] [nvarchar](max) NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonContactInfo] PRIMARY KEY CLUSTERED 
(
	[ContactInfoTypeId] ASC,
	[PersonId] ASC
)
)
