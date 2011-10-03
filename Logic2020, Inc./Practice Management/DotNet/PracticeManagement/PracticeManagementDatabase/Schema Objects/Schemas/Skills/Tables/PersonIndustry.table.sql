CREATE TABLE [Skills].[PersonIndustry](
	[IndustryId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[YearsExperience] [int] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModificationDate] [datetime] NOT NULL,
	[TenantId]		[int] NOT NULL,
 CONSTRAINT [PK_PersonIndustry] PRIMARY KEY CLUSTERED 
(
	[TenantId] ASC,
	[IndustryId] ASC,
	[PersonId] ASC
)
)
