ALTER TABLE [dbo].[Milestone]
    ADD CONSTRAINT [DF_Milestone_ConsultantCanAdjust] DEFAULT ((0)) FOR [ConsultantsCanAdjust];


