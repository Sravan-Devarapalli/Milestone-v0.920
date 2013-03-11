-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 30-05-2012
-- Updated by:	ThulasiRam.P
-- Update date: 30-05-2012
-- Description:	Updates a PersonCalendarAuto table according to the data in the Calendar and PersonCalendar table
-- =============================================
CREATE TRIGGER [dbo].[tr_PersonCalendar_InsertUpdateDelete]
ON [dbo].[PersonCalendar]
AFTER INSERT, UPDATE , DELETE
AS
BEGIN
	SET NOCOUNT ON;

	 -- IF Delete Fires 
   
   IF NOT EXISTS (SELECT 1 FROM INSERTED)
   BEGIN
    
    UPDATE PCA
	SET PCA.DayOff = cal.DayOff,
		PCA.TimeOffHours = NULL
	FROM DELETED AS d
	INNER JOIN dbo.PersonCalendarAuto AS PCA ON d.Date = PCA.Date  and d.PersonId = PCA.PersonId
	INNER JOIN dbo.Calendar AS cal ON cal.Date = PCA.Date 
	WHERE PCA.DayOff <> cal.DayOff

   END
   -- IF UPDATE OR INSERT FIRES 
   ELSE 
   BEGIN

    UPDATE PCA
	SET PCA.DayOff = cal.DayOff,
		PCA.TimeOffHours = NULL
	FROM DELETED AS d
	INNER JOIN dbo.PersonCalendarAuto AS PCA ON d.Date = PCA.Date  and d.PersonId = PCA.PersonId
	INNER JOIN dbo.Calendar AS cal ON cal.Date = PCA.Date 
	WHERE PCA.DayOff <> cal.DayOff OR ISNULL(PCA.TimeOffHours,0) <> ISNULL(d.ActualHours,0)
   
    UPDATE PCA
	SET PCA.DayOff = i.DayOff,
		PCA.TimeOffHours = i.ActualHours
	FROM INSERTED AS i
	INNER JOIN dbo.PersonCalendarAuto AS PCA ON i.Date = PCA.Date  and i.PersonId = PCA.PersonId
	INNER JOIN dbo.Calendar AS cal ON cal.Date = PCA.Date 
	WHERE PCA.DayOff <> i.DayOff OR ISNULL(PCA.TimeOffHours,0) <> ISNULL(i.ActualHours,0)
	
   END

   IF NOT EXISTS (	SELECT 1 
					FROM inserted i 
					INNER JOIN deleted d ON i.Date = d.Date 
											AND i.PersonId = d.PersonId
											AND i.ActualHours = d.ActualHours
											AND ISNULL(i.ApprovedBy,'') = ISNULL(d.ApprovedBy,'')
											AND i.DayOff = d.DayOff
											AND i.Description = d.Description
											AND ISNULL(i.SubstituteDate,'') = ISNULL(d.SubstituteDate,'')
											AND i.TimeTypeId = d.TimeTypeId
											AND i.SeriesId != d.SeriesId
				)
	BEGIN

	   ---- Below Code is for Update SeriesID column

		DECLARE @MaxSeriesIdValue BIGINT = 0
		SELECT @MaxSeriesIdValue = MAX(PC.SeriesId)
		FROM dbo.PersonCalendar as PC

	   ;WITH SeriesIDList AS
	   ( 
	   SELECT  i.SeriesID 
	   FROM inserted as i 
	   UNION 
	   SELECT  d.SeriesID 
	   FROM deleted as d 
	   ),
		GroupedPersonCalendar AS 
		(
			SELECT  pc.PersonId, 
					ISNULL(pc.TimeTypeId,0) AS TimeTypeId, 
					pc.DayOff,
					pc.Date,
					ISNULL(pc.ActualHours,0) AS ActualHours,
					DATEADD([day],
					(-1 * 
					(
					 DENSE_RANK() OVER(PARTITION BY  pc.PersonId,  pc.TimeTypeId, pc.DayOff , pc.ActualHours ORDER BY pc.date) /* Dense Rank */ 
					 + 
					 (
					 -2 /* saturday +sunday */
					 * 
					 DATEDIFF(WEEK,
							  (DATEADD(dd, -(DATEPART(dw, pc.date)-1),  pc.date)) /* Gives sunday date for pc.date */,
							   MIN(
							   DATEADD(dd, -(DATEPART(dw, pc.date)-1),  pc.date)  /* Gives sunday date for pc.date */
								) OVER(PARTITION BY  pc.PersonId,  pc.TimeTypeId, pc.DayOff , pc.ActualHours ) /* Gives minimum sudaydate  in pertitioned data*/
							 ) /* Gives week difference BETWEEN   (sunday date for current row pc.date) AND (minimum sundaydate  in partitioned data) */
				
					)
				
					)) ,pc.date) as GroupedDate
			FROM  dbo.PersonCalendar AS pc
			INNER JOIN SeriesIDList AS  SIDL ON  SIDL.SeriesId = PC.SeriesId
		),
		GroupedPersonCalendarWithSeriesID AS
		(
		SELECT  ( @MaxSeriesIdValue + ROW_NUMBER()OVER(ORDER BY pc.PersonId,pc.TimeTypeId, pc.DayOff , pc.ActualHours)) AS SeriesId,
				pc.PersonId,  
				pc.TimeTypeId, 
				pc.DayOff , 
				pc.ActualHours,
				MIN(pc.Date) AS SeriesStartDate,
				MAX(pc.Date) AS SeriesEndDate
		FROM GroupedPersonCalendar AS pc
		GROUP BY pc.PersonId,  pc.TimeTypeId, pc.DayOff , pc.ActualHours ,GroupedDate
		)


		UPDATE PC
		SET PC.SeriesId = TPC.SeriesId 
		FROM dbo.PersonCalendar AS PC
		INNER JOIN SeriesIDList AS  SIDL ON  SIDL.SeriesId = PC.SeriesId
		INNER JOIN GroupedPersonCalendarWithSeriesID AS TPC ON PC.PersonId = TPC.PersonId AND ISNULL(PC.TimeTypeId,0)=TPC.TimeTypeId AND  Pc.DayOff = tpc.DayOff AND ISNULL(pc.ActualHours,0) = tpc.ActualHours 
																AND pc.Date BETWEEN Tpc.SeriesStartDate AND TPC.SeriesEndDate

	END 

END

