ALTER TABLE [dbo].[Note]
    ADD CONSTRAINT [DF_Note] DEFAULT (getdate()) FOR [CreateDate];


