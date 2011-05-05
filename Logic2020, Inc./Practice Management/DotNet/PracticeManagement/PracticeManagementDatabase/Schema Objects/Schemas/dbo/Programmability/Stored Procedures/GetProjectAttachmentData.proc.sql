CREATE PROCEDURE dbo.GetProjectAttachmentData
(
	@ProjectId	         INT
)
AS
BEGIN
	SELECT TOP(1) pa.AttachmentData 
	FROM dbo.ProjectAttachment AS pa
	WHERE pa.ProjectId = @ProjectId
END

