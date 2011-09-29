CREATE TABLE [dbo].[PersonIndustry](
	[IndustryId] [int] NOT NULL,
	[PersonId] [bigint] NOT NULL,
	[YearsExperience] [int] NOT NULL,
	[DisplayOrder] [tinyint] NULL,
	[ModificationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_PersonIndustry] PRIMARY KEY CLUSTERED 
(
	[IndustryId] ASC,
	[PersonId] ASC
)
)
