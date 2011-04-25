
-- =============================================
-- Author:		Artem Dolya
-- Create date: 2010-05-13
-- Description:	Inserts new email template record
-- =============================================
CREATE PROCEDURE dbo.EmailTemplateUpdate
	@EmailTemplateId int,
    @EmailTemplateName nvarchar(50) ,
	@EmailTemplateTo nvarchar(250) = NULL ,
	@EmailTemplateCc nvarchar(250) = NULL ,
    @EmailTemplateSubject nvarchar(255) ,
    @EmailTemplateBody nvarchar(MAX)
AS 
    BEGIN
        SET NOCOUNT ON ;
	
		IF NOT EXISTS (
					SELECT 1 
					FROM dbo.EmailTemplate et
					WHERE NOT et.EmailTemplateId = @EmailTemplateId 
					       AND et.[EmailTemplateName] = @EmailTemplateName)
		BEGIN 
			UPDATE dbo.EmailTemplate SET
                   [EmailTemplateName] = @EmailTemplateName ,
				   [EmailTemplateTo] = @EmailTemplateTo ,
				   [EmailTemplateCc] = @EmailTemplateCc ,
                   [EmailTemplateSubject] = @EmailTemplateSubject,
                   [EmailTemplateBody] = @EmailTemplateBody
             WHERE EmailTemplateId = @EmailTemplateId
		END
		ELSE
		BEGIN
			RAISERROR (N'Email template with specified name already exist!',16, 1);
		END
    END

