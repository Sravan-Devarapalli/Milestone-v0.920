CREATE PROCEDURE dbo.GetProjectAttachmentData
(
	@ProjectId	         INT,
	@AttachmentId		 INT
)
AS
BEGIN
	SELECT TOP(1) pa.AttachmentData 
	FROM dbo.ProjectAttachment AS pa
	WHERE pa.ProjectId = @ProjectId
		AND pa.Id = @AttachmentId
END

