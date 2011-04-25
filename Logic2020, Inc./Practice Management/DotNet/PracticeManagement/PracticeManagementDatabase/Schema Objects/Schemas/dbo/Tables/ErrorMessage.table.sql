CREATE TABLE [dbo].[ErrorMessage]
(
	MessageId	INT NOT NULL,
	LanguageId	SMALLINT,
	Severity	TINYINT,
	MessageText	NVARCHAR(2048)
 )
