ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [CK_Project_StartEndDate] CHECK ([EndDate]>=[StartDate]);


