ALTER TABLE [dbo].[DefaultRecruiterCommissionHeader]
    ADD CONSTRAINT [CK_DefaultRecruiterCommissionHeader_Period] CHECK ([EndDate]>[StartDate]);


