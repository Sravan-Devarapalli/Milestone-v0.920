CREATE TRIGGER [tr_Vendor_Log]
	ON [dbo].[Vendor]
	AFTER INSERT, UPDATE ,DELETE
AS 
BEGIN
  -- Ensure the temporary table exists
	EXEC SessionLogPrepare @UserLogin = NULL
	DECLARE @CurrentPMTime DATETIME 
	SET @CurrentPMTime = dbo.InsertingTime()

	;WITH NEW_VALUES AS
	(
		SELECT i.Id as 'VendorId',
				i.Name,
				i.ContactName,
				i.Email,
				i.Status,
				i.TelephoneNumber,
				VT.Name as 'VendorType'
		  FROM inserted AS i
		  INNER JOIN dbo.VendorType VT ON VT.Id = i.VendorTypeId
	),

	OLD_VALUES AS
	(
		SELECT d.Id 'VendorId',
				d.Name,
				d.ContactName,
				d.Email,
				d.Status,
				d.TelephoneNumber,
				VT.Name as 'VendorType'
		  FROM deleted AS d
		  INNER JOIN dbo.VendorType VT ON VT.Id = d.VendorTypeId
	)

	-- Log an activity
	INSERT INTO dbo.UserActivityLog
	            (ActivityTypeID,
	             SessionID,
	             SystemUser,
	             Workstation,
	             ApplicationName,
	             UserLogin,
	             PersonID,
	             LastName,
	             FirstName,
				 Data,
	             LogData,
	             LogDate)
	SELECT  CASE
	           WHEN d.Id IS NULL THEN 3
	           WHEN i.Id IS NULL THEN 5
	           ELSE 4
	       END as ActivityTypeID,
	       l.SessionID,
	       l.SystemUser,
	       l.Workstation,
	       l.ApplicationName,
	       l.UserLogin,
	       l.PersonID,
	       l.LastName,
	       l.FirstName,
	      Data = CONVERT(NVARCHAR(MAX),(SELECT *
					    FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.VendorId = OLD_VALUES.VendorId
			           WHERE NEW_VALUES.VendorId = ISNULL(i.Id, d.Id) OR OLD_VALUES.VendorId = ISNULL(i.Id, d.Id)
					  FOR XML AUTO, ROOT('Vendor'))),
		LogData = (SELECT 
						NEW_VALUES.VendorId 
						,NEW_VALUES.Name
						,NEW_VALUES.ContactName
						,NEW_VALUES.Email
						,NEW_VALUES.Status 
						,NEW_VALUES.TelephoneNumber
						,NEW_VALUES.VendorType
						,OLD_VALUES.VendorId 
						,OLD_VALUES.Name
						,OLD_VALUES.ContactName
						,OLD_VALUES.Email
						,OLD_VALUES.Status 
						,OLD_VALUES.TelephoneNumber
						,OLD_VALUES.VendorType
					  FROM NEW_VALUES
					         FULL JOIN OLD_VALUES ON NEW_VALUES.VendorId = OLD_VALUES.VendorId
			           WHERE NEW_VALUES.VendorId = ISNULL(i.Id, d.Id) OR OLD_VALUES.VendorId = ISNULL(i.Id, d.Id)
					  FOR XML AUTO, ROOT('Vendor'), TYPE),
			@CurrentPMTime
	  FROM inserted AS i
	       FULL JOIN deleted AS d ON i.Id = d.Id
	       INNER JOIN dbo.SessionLogData AS l ON l.SessionID = @@SPID
	  WHERE ISNULL(i.Name, '') <> ISNULL(d.Name, '')
		  OR ISNULL(i.ContactName, '') <> ISNULL(d.ContactName,'')
		  OR ISNULL(i.Email, '') <> ISNULL(d.Email, '')
		  OR ISNULL(i.Status, 0) <> ISNULL(d.Status, 0)
		  OR ISNULL(i.TelephoneNumber, 0) <> ISNULL(d.TelephoneNumber, 0)
		  OR ISNULL(i.VendorTypeId, 0) <> ISNULL(d.VendorTypeId, 0)

	 -- End logging session
	EXEC dbo.SessionLogUnprepare
END

