CREATE FUNCTION GetDailyDemand
(
	@StartDate DATETIME,
	@EndDate DATETIME,
	@StrawmanId INT,
	@ObjectId INT,
	@ObjectTypeId INT
)
RETURNS NVARCHAR(4000)
AS
BEGIN
	
	DECLARE @DaysForward INT ,
		@Index INT = 0,
		@QuantityString NVARCHAR(4000) = ''


	DECLARE @value INT
		
	SELECT @DaysForward = DATEDIFF(DD,@StartDate,@EndDate)+1
		-- Iterate through all days
		WHILE ( @Index <= @DaysForward)
		BEGIN
			DECLARE @Date DATETIME = DATEADD(DD, @Index, @StartDate)

			SET @value = 0
			
			IF @ObjectTypeId = 2
			BEGIN

				SELECT @value = COUNT(MPE.Id)
				FROM Project P
				JOIN Milestone M ON M.ProjectId = P.ProjectId
				JOIN MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
				JOIN MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
				WHERE MP.PersonId = @StrawmanId AND P.ProjectId = @ObjectId
					AND @Date = MPE.StartDate--AND @Date BETWEEN MPE.StartDate AND MPE.EndDate
				GROUP BY P.ProjectId

			END
			ELSE
			BEGIN

				SELECT @value = OP.Quantity
				FROM dbo.OpportunityPersons OP
				JOIN dbo.Opportunity O ON O.OpportunityId = OP.OpportunityId
				WHERE OP.RelationTypeId = 2 -- Team Structure
					AND OP.PersonId = @StrawmanId AND OP.OpportunityId = @ObjectId
					AND @Date = OP.NeedBy--AND @Date BETWEEN OP.NeedBy AND O.ProjectedEndDate
				
			END
						
			SET @QuantityString  = @QuantityString + CONVERT( NVARCHAR, ISNULL(@value, 0)) + ','
			
			SET @Index = @Index + 1
			
		END

	RETURN @QuantityString

END
