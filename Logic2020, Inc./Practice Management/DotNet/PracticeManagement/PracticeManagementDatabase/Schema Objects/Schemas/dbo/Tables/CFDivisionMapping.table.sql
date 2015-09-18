CREATE TABLE [dbo].[CFDivisionMapping]
(
   Id			INT		IDENTITY (1, 1) NOT NULL,
   DivisionId	INT		NOT NULL,
   PracticeId	INT		NOT NULL,
   CFDivisionId	INT		NOT NULL,
   TitleId		INT		NULL
)


