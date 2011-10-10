CREATE TABLE [Skills].[PersonIndustry](
	[IndustryId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
	[YearsExperience] [int] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModifiedDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonIndustry] PRIMARY KEY CLUSTERED 
(
	[IndustryId] ASC,
	[PersonId] ASC
)
)
