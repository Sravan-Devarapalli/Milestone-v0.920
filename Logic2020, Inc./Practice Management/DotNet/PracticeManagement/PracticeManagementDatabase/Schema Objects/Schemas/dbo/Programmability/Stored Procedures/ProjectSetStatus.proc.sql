----##############################################################################
---- CREATE NEW STORED PROCEDURES
----##############################################################################
CREATE PROCEDURE [dbo].[ProjectSetStatus] 
 @ProjectID int,
 @ProjectStatusId int
AS
BEGIN
 SET NOCOUNT ON;
  
 UPDATE Project
  SET ProjectStatusId = @ProjectStatusId
  WHERE ProjectID = @ProjectID
END
