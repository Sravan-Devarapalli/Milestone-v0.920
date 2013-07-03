CREATE PROCEDURE [dbo].[PersonDelete]
(
	@PersonId   INT
)
AS
	SET NOCOUNT ON

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

	DELETE dbo.PersonHistory
	 WHERE PersonId = @PersonId

