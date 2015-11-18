CREATE PROCEDURE [dbo].[GetManagedParametersByPerson]
(
	@UserLogin			  NVARCHAR(255)
)
AS
BEGIN

	DECLARE @PersonId INT
	SELECT @PersonId = PersonId FROM dbo.Person WHERE Alias = @UserLogin

	SELECT ActualRevenuePerHour,
		   TargetRevenuePerHour,
		   HoursUtilization,
		   TargetRevenuePerAnnum 
	FROM dbo.ManagedParametersByPerson
	WHERE  PersonId=@PersonId

END
