CREATE PROCEDURE [dbo].[EmailTemplateGetByName]
(
	@EmailTemplateName	NVARCHAR(50)
)
AS
BEGIN
	SET NOCOUNT ON;
	DECLARE @EmailTemplateNameLocal NVARCHAR(50)
	SELECT @EmailTemplateNameLocal = @EmailTemplateName
	SELECT 
			et.EmailTemplateId
			, et.EmailTemplateName
			, et.EmailTemplateTo
			, et.EmailTemplateCc
			, et.EmailTemplateSubject
			, et.EmailTemplateBody
		FROM EmailTemplate AS et
		WHERE ISNULL(et.Name,et.EmailTemplateName) = @EmailTemplateNameLocal
END

