ALTER TABLE [dbo].[Project]
    ADD CONSTRAINT [DF_Project_ProjectManager] DEFAULT ((1)) FOR [ProjectManagerId];


