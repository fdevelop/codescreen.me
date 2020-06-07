CREATE TABLE [dbo].[CodeSessions] (
    [Id]             NVARCHAR (20)  NOT NULL,
    [DateCreated]    DATETIME2 (7)  NOT NULL,
    [Code]           NVARCHAR (MAX) NULL,
    [CodeSyntax]     NVARCHAR (100) NULL,
    [CodeHighlights] NVARCHAR (MAX) NULL,
    [Owner]          NVARCHAR (100) NULL,
    [UserInControl]  NVARCHAR (100) NULL,
    [Participants]   NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_CodeSessions] PRIMARY KEY CLUSTERED ([Id] ASC)
);

