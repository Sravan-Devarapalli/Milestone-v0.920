CREATE TABLE [dbo].[EmailTemplate] (
    [EmailTemplateId]      INT            IDENTITY (1, 1) NOT NULL,
    [EmailTemplateName]    NVARCHAR (50)  NOT NULL,
    [EmailTemplateTo]    NVARCHAR (250),
    [EmailTemplateCc]    NVARCHAR (250),
    [EmailTemplateSubject] NVARCHAR (255) NOT NULL,
    [EmailTemplateBody]    NVARCHAR (MAX) NOT NULL
);


