CREATE TABLE [dbo].[PersonContactInfo](
	[ContactInfoTypeId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[Info] [nvarchar](max) NULL,
	[ModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonContactInfo] PRIMARY KEY CLUSTERED 
(
	[ContactInfoTypeId] ASC,
	[PersonId] ASC
)
)
