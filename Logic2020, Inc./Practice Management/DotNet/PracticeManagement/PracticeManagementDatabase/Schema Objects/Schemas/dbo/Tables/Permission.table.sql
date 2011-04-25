CREATE TABLE [dbo].[Permission] (
    [PermissionId] INT      IDENTITY (1, 1) NOT NULL,
    [PersonId]     INT      NOT NULL,
    [TargetType]   SMALLINT NOT NULL,
    [TargetId]     INT      NULL
);


