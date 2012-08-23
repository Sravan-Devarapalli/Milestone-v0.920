CREATE TRIGGER [tr_RecruiterCommission_InsertUpdate]
    ON [dbo].[RecruiterCommission]
    FOR INSERT, UPDATE 
    AS 
    BEGIN
    	SET NOCOUNT ON;

		--IF EXISTS (SELECT *
		--			FROM RecruiterCommissionHistory RCH
		--			)
		--INSERT INTO RecruiterCommissionHistory(    [RecruitId],
		--											[RecruiterId],
		--											[HoursToCollect],
		--											[Amount],
		--											[StartDate],
		--											[EndDate]
		--										)
		--VALUES

		DECLARE @Today DATETIME

		SELECT @Today = CONVERT(DATETIME,CONVERT(DATE, dbo.InsertingTime()))

		UPDATE RCH
			SET Amount = I.Amount,
				HoursToCollect = I.HoursToCollect
		FROM inserted I
		INNER JOIN RecruiterCommissionHistory RCH ON RCH.RecruitId = I.RecruitId AND RCH.RecruiterId = I.RecruiterId AND RCH.EndDate IS NULL
		WHERE RCH.Amount <> I.Amount AND RCH.HoursToCollect <> I.HoursToCollect
		
		--If exists with other recruiter then insert close the existing one and insert the new one.
		Update RCH
		SET EndDate = @Today - 1
		FROM RecruiterCommissionHistory RCH
		INNER JOIN inserted I ON I.RecruitId = RCH.RecruitId AND I.RecruiterId <> RCH.RecruiterId AND RCH.EndDate IS NULL

		--If not exists then insert with start date as hiredate/today and enddate as NULL.
		INSERT INTO RecruiterCommissionHistory(    [RecruitId],
													[RecruiterId],
													[HoursToCollect],
													[Amount],
													[StartDate]
												)
		SELECT I.RecruitId,
				I.RecruiterId,
				I.HoursToCollect,
				I.Amount,
				CASE WHEN PH.StartDate < @Today THEN PH.StartDate
					ELSE @Today END
		FROM inserted I
		OUTER APPLY (SELECT top 1 * FROM PersonStatusHistory PSH WHERE PSH.PersonId = I.RecruitId ORDER BY PSH.StartDate) PH
		LEFT JOIN RecruiterCommissionHistory RCH ON RCH.RecruitId = I.RecruitId AND I.RecruiterId = RCH.RecruiterId AND RCH.EndDate IS NULL
		WHERE I.RecruitId IS NULL

    END
