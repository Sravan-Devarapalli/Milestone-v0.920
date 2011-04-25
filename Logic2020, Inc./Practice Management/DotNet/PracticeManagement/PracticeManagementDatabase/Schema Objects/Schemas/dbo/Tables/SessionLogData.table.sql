CREATE TABLE dbo.SessionLogData
(
	SessionID         INT NOT NULL,
	UserLogin         NVARCHAR(255) NULL,
	PersonID          INT NULL,
	LastName          NVARCHAR(100) NULL,
	FirstName         NVARCHAR(100) NULL,
	SystemUser        NVARCHAR(255) NOT NULL DEFAULT SYSTEM_USER,
	Workstation       NVARCHAR(128) NULL DEFAULT HOST_NAME(),
	ApplicationName   NVARCHAR(128) NULL DEFAULT APP_NAME(),
	CONSTRAINT PK_SessionLogData PRIMARY KEY CLUSTERED (SessionID)
)
