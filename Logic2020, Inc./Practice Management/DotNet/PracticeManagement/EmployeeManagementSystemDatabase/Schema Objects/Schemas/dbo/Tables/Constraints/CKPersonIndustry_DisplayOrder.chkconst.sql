﻿ALTER TABLE [dbo].[PersonIndustry]  WITH CHECK ADD  CONSTRAINT [CKPersonIndustry_DisplayOrder] CHECK  (([DisplayOrder]>=(1) AND [DisplayOrder]<=(20)))
GO
ALTER TABLE [dbo].[PersonIndustry] CHECK CONSTRAINT [CKPersonIndustry_DisplayOrder]
GO
