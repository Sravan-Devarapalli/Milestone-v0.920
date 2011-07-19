CREATE TABLE [dbo].[UserPassWordHistory] (
     Id										 INT IDENTITY(1,1) NOT NULL,	
    [UserId]                                 UNIQUEIDENTIFIER NOT NULL,
    [Password]                               NVARCHAR (128)   NOT NULL,
    [PasswordFormat]                         INT             CONSTRAINT Df_UserPassWordHistory_PasswordFormat DEFAULT ((0)) NOT NULL,
    [PasswordSalt]                           NVARCHAR (128)   NOT NULL,
    [LastPasswordChangedDate]                DATETIME         NOT NULL,
    CONSTRAINT PK_UserPassWordHistory_Id     PRIMARY KEY CLUSTERED(Id),
    CONSTRAINT FK_UserPassWordHistory_UserId FOREIGN KEY(UserId) REFERENCES  dbo.aspnet_Membership(UserId)
);
