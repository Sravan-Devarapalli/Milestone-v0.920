CREATE PROCEDURE dbo.MilestoneResourceUpdate
(
	@MilestoneId              INT,
	@StartDate                DATETIME,
	@ProjectedDeliveryDate    DATETIME,
	@IsStartDateChangeReflectedForMilestoneAndPersons BIT,
	@IsEndDateChangeReflectedForMilestoneAndPersons   BIT
)
AS
BEGIN
	SET NOCOUNT ON;

		DECLARE @TempTable TABLE(MilestonePersonId INT)
		
		INSERT INTO @TempTable
		SELECT   mp.MilestonePersonId
		FROM [dbo].[MilestonePerson] AS mp
		INNER JOIN dbo.Person AS p ON mp.PersonId = p.PersonId
		WHERE  (mp.MilestoneId = @MilestoneId) 
		  AND  (
			    (p.TerminationDate < @StartDate) 
			    OR (p.HireDate > @ProjectedDeliveryDate)
			   )
	
		 
		DELETE MPE FROM dbo.MilestonePersonEntry MPE
		JOIN @TempTable Temp ON Temp.MilestonePersonId = MPE.MilestonePersonId

		 
		DELETE MP FROM dbo.MilestonePerson MP
		JOIN @TempTable Temp ON Temp.MilestonePersonId = MP.MilestonePersonId

		DECLARE @TempMPE TABLE
		(MilestonePersonId INT,--StartDate DATETIME,EndDate DATETIME,
		PersonRoleId INT,Amount DECIMAL(18,2),HoursPerDay DECIMAL(4,2))
		
		INSERT INTO @TempMPE(MilestonePersonId,PersonRoleId ,Amount ,HoursPerDay )
		SELECT MPE.MilestonePersonId,MPE.PersonRoleId,MAX(MPE.Amount),MAX(MPE.HoursPerDay)
		FROM dbo.MilestonePersonEntry MPE
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
		WHERE MP.MilestoneId = @MilestoneId 
			AND ( MPE.StartDate > @ProjectedDeliveryDate OR MPE.EndDate < @StartDate )
		GROUP BY MPE.MilestonePersonId,MPE.PersonRoleId
		
		
		DELETE  MPE FROM dbo.MilestonePersonEntry MPE
		JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
		WHERE MP.MilestoneId = @MilestoneId 
			AND ( MPE.StartDate > @ProjectedDeliveryDate OR MPE.EndDate < @StartDate )
			
		INSERT INTO dbo.MilestonePersonEntry(MilestonePersonId,StartDate,EndDate,PersonRoleId,Amount,Location,HoursPerDay )
		SELECT TEMPmpe.MilestonePersonId,@StartDate,@StartDate,TEMPmpe.PersonRoleId,TEMPmpe.Amount, NULL, TEMPmpe.HoursPerDay  
		FROM @TempMPE AS TEMPmpe
		WHERE NOT EXISTS (SELECT 1 FROM dbo.MilestonePersonEntry MPE
						 WHERE  TEMPmpe.MilestonePersonId = MPE.MilestonePersonId 
								AND TEMPmpe.PersonRoleId = MPE.PersonRoleId)
		
		DELETE MP FROM dbo.MilestonePerson MP
		LEFT JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
		WHERE MPE.MilestonePersonId IS NULL AND MP.MilestoneId = @MilestoneId


	    UPDATE mpentry
			   SET StartDate = CASE
									 WHEN ( P.HireDate > @StartDate) THEN  P.HireDate
									 ELSE @StartDate
								   END
								
			  FROM MilestonePersonEntry as mpentry
			  INNER JOIN 
						(
						 SELECT RANK() OVER(PARTITION BY  mpe.MilestonePersonId,ISNULL(mpe.PersonRoleId,0) ORDER BY StartDate) AS RANKnO,
								mpe.Id,
								mp.MilestoneId,
								mp.PersonId
						 FROM dbo.MilestonePersonEntry AS mpe
						 INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
						 ) AS q ON mpentry.Id = q.Id
			  INNER JOIN dbo.Person AS p ON p.PersonId = q.PersonId

			  WHERE 1 = q.RANKnO AND q.MilestoneId = @MilestoneId  AND @IsStartDateChangeReflectedForMilestoneAndPersons = 1 

		UPDATE mpentry
				SET EndDate =  CASE
									WHEN ( @ProjectedDeliveryDate > p.TerminationDate ) THEN  p.TerminationDate
									ELSE (@ProjectedDeliveryDate)
								  END
			    FROM MilestonePersonEntry as mpentry
				INNER JOIN 
					(
					 SELECT RANK() OVER(PARTITION BY  mpe.MilestonePersonId,ISNULL(mpe.PersonRoleId,0) ORDER BY EndDate DESC) AS RANKnO,
							mpe.Id,
							mp.MilestoneId,
							mp.PersonId
					 FROM dbo.MilestonePersonEntry AS mpe
					 INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
					 ) AS q ON mpentry.Id = q.Id
				INNER JOIN dbo.Person AS p ON p.PersonId = q.PersonId
				WHERE  1 = q.RANKnO AND q.MilestoneId = @MilestoneId AND  @IsEndDateChangeReflectedForMilestoneAndPersons = 1 

	    UPDATE mpe
			   SET EndDate =  CASE WHEN @IsStartDateChangeReflectedForMilestoneAndPersons =1 THEN 
								( CASE
									 WHEN ( mpe.StartDate > mpe.EndDate) THEN  mpe.StartDate
									 ELSE mpe.EndDate
								   END
								) ELSE mpe.EndDate END,
				  StartDate = CASE WHEN @IsEndDateChangeReflectedForMilestoneAndPersons =1 THEN 
								( CASE
									 WHEN ( mpe.StartDate > mpe.EndDate) THEN  mpe.EndDate
									 ELSE mpe.StartDate
								   END
								) ELSE mpe.StartDate END
			  FROM dbo.MilestonePersonEntry AS mpe
				   INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
				   INNER JOIN dbo.Person AS p ON p.PersonId = mp.PersonId
			 WHERE mp.MilestoneId = @MilestoneId AND (@IsStartDateChangeReflectedForMilestoneAndPersons = 1 OR @IsEndDateChangeReflectedForMilestoneAndPersons = 1 )

 END
