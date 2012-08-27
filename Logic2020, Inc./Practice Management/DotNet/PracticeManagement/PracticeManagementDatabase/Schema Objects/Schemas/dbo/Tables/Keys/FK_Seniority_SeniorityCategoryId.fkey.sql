ALTER TABLE [dbo].[Seniority]
  ADD CONSTRAINT [FK_Seniority_SeniorityCategoryId] FOREIGN KEY ([SeniorityCategoryId]) REFERENCES [dbo].[SeniorityCategory] ([SeniorityCategoryId]) ON DELETE NO ACTION ON UPDATE NO ACTION;



