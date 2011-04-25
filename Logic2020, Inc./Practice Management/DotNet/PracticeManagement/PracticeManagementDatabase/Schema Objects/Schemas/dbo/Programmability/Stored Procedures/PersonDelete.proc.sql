-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-10-2008
-- Updated by:	
-- Update date: 
-- Description:	Deletes a person from the database.
-- =============================================
CREATE PROCEDURE [dbo].[PersonDelete]
(
	@PersonId   INT
)
AS
	SET NOCOUNT ON

	DELETE dbo.DefaultCommission
	 WHERE PersonId = @PersonId

	DELETE dbo.Pay
	 WHERE Person = @PersonId

	DELETE dbo.PersonCalendar
	 WHERE PersonId = @PersonId

	DELETE dbo.PersonCalendarAuto
	 WHERE PersonId = @PersonId

	 DELETE dbo.PersonStatusHistory
	 WHERE PersonId = @PersonId

	DELETE dbo.Person
	 WHERE PersonId = @PersonId

