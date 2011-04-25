CREATE PROCEDURE [dbo].[EmailTemplateGetByName]
(
	@EmailTemplateName	nvarchar(50)
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @EmailTemplateNameLocal nvarchar(50)
	SELECT @EmailTemplateNameLocal = @EmailTemplateName
	Select 
			et.EmailTemplateId
			, et.EmailTemplateName
			, et.EmailTemplateTo
			, et.EmailTemplateCc
			, et.EmailTemplateSubject
			, et.EmailTemplateBody
		FROM EmailTemplate AS et
		WHERE et.EmailTemplateName = @EmailTemplateNameLocal
END

