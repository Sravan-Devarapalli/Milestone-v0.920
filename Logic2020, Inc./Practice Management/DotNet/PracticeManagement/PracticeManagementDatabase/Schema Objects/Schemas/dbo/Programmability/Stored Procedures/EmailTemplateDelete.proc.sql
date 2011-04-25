
-- =============================================
-- Author:		Artem Dolya
-- Create date: 2010-05-13
-- Description:	Inserts new email template record
-- =============================================
CREATE PROCEDURE dbo.EmailTemplateDelete
	@EmailTemplateId int
AS 
    BEGIN
        SET NOCOUNT ON ;
	
		DELETE dbo.EmailTemplate
		 WHERE EmailTemplateId = @EmailTemplateId
      
    END

