CREATE PROCEDURE [dbo].[SaveManagedParametersByPerson]
(
	@UserLogin			  NVARCHAR(255),
	@ActualRevenuePerHour DECIMAL(32,2),
	@TargetRevenuePerHour DECIMAL(32,2),
	@HoursUtilization DECIMAL(32,2),
	@TargetRevenuePerAnnum DECIMAL(32,2)
)
AS 
BEGIN
	DECLARE @PersonId INT
	SELECT @PersonId = PersonId FROM dbo.Person WHERE Alias = @UserLogin

	IF EXISTS(SELECT * FROM dbo.ManagedParametersByPerson WHERE PersonId=@PersonId)
	BEGIN 
		UPDATE dbo.ManagedParametersByPerson 
		SET ActualRevenuePerHour=@ActualRevenuePerHour,
			TargetRevenuePerHour=@TargetRevenuePerHour,
			HoursUtilization=@HoursUtilization,
			TargetRevenuePerAnnum=@TargetRevenuePerAnnum
		WHERE PersonId=@PersonId
	END
	ELSE
	BEGIN
		INSERT INTO dbo.ManagedParametersByPerson (PersonId,ActualRevenuePerHour,TargetRevenuePerHour,HoursUtilization,TargetRevenuePerAnnum)
		VALUES(@PersonId,@ActualRevenuePerHour,@TargetRevenuePerHour,@HoursUtilization,@TargetRevenuePerAnnum)
	END
END

