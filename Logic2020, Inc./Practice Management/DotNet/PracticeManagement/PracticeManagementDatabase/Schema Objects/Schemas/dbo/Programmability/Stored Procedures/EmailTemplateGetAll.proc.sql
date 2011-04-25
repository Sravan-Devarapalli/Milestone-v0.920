
CREATE PROCEDURE dbo.EmailTemplateGetAll
AS
BEGIN
	SET NOCOUNT ON;
	
	Select 
			et.EmailTemplateId
			, et.EmailTemplateName
			, et.EmailTemplateTo
			, et.EmailTemplateCc
			, et.EmailTemplateSubject
			, et.EmailTemplateBody
		FROM EmailTemplate AS et
		ORDER BY et.EmailTemplateName
	
END


