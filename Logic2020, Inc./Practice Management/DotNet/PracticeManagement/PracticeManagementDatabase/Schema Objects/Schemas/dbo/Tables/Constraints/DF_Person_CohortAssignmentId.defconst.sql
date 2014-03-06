ALTER TABLE [dbo].[Person]
   ADD CONSTRAINT [DF_Person_CohortAssignmentId] 
   DEFAULT 1
   FOR CohortAssignmentId
