-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[Users] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Username] NVARCHAR(50) NOT NULL UNIQUE,
    [PasswordHash] NVARCHAR(255) NOT NULL,
    [Email] NVARCHAR(100) NOT NULL UNIQUE,
    [Role] NVARCHAR(20) NOT NULL DEFAULT 'User',
    [CreatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE())
);

-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[Surveys] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [Title] NVARCHAR(255) NOT NULL,
    [CreatedBy] INT NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT FK_Surveys_CreatedBy FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[Users]([Id])
);


-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.
CREATE TABLE [dbo].[Options] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [SurveyId] INT NOT NULL,
    [Text] NVARCHAR(255) NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE()),
    CONSTRAINT FK_Options_SurveyId FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys]([Id]) ON DELETE CASCADE,
    CONSTRAINT UQ_Options_Survey_Text UNIQUE([SurveyId], [Text])
);



-- This script only contains the table creation statements and does not fully represent the table in the database. It's still missing: sequences, indices, triggers. Do not use it as a backup.

CREATE TABLE [dbo].[Votes] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,
    [UserId] INT NOT NULL,
    [SurveyId] INT NOT NULL,
    [OptionId] INT NOT NULL,
    [CreatedAt] DATETIME NOT NULL DEFAULT (GETUTCDATE()),

    CONSTRAINT FK_Votes_User FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users]([Id]),
    CONSTRAINT FK_Votes_Survey FOREIGN KEY ([SurveyId]) REFERENCES [dbo].[Surveys]([Id]),
    CONSTRAINT FK_Votes_Option FOREIGN KEY ([OptionId]) REFERENCES [dbo].[Options]([Id]),
    CONSTRAINT UQ_Votes_UniqueVote UNIQUE ([UserId], [SurveyId])
);

-- SEED: Insert test user (admin2)
SET IDENTITY_INSERT [dbo].[Users] ON;

INSERT INTO [dbo].[Users] ([Id], [Username], [PasswordHash], [Email], [Role], [CreatedAt])
VALUES (
  5, 
  'admin2', 
  '$2a$11$PjIzQIQ.yjSK7jRJWSPZQOWYQ4f3EOX1HNNtLZ/3iRaDRgFRvW7OS',  -- bcrypt hash for 'admin1234'
  'admin2@example.com', 
  'Admin', 
  GETUTCDATE()
);

SET IDENTITY_INSERT [dbo].[Users] OFF;

-- SEED: Insert main admin user
SET IDENTITY_INSERT [dbo].[Users] ON;

INSERT INTO [dbo].[Users] ([Id], [Username], [PasswordHash], [Email], [Role], [CreatedAt])
VALUES (
  1,
  'admin',
  '$2a$11$PjIzQIQ.yjSK7jRJWSPZQOWYQ4f3EOX1HNNtLZ/3iRaDRgFRvW7OS', -- same bcrypt for 'admin1234'
  'admin@example.com',
  'Admin',
  GETUTCDATE()
);

SET IDENTITY_INSERT [dbo].[Users] OFF;

-- SEED: Insert test survey
SET IDENTITY_INSERT [dbo].[Surveys] ON;
INSERT INTO [dbo].[Surveys] ([Id], [Title], [CreatedBy], [CreatedAt])
VALUES (1, 'Sample Survey', 5, GETUTCDATE());
SET IDENTITY_INSERT [dbo].[Surveys] OFF;

-- SEED: Insert test options
SET IDENTITY_INSERT [dbo].[Options] ON;
INSERT INTO [dbo].[Options] ([Id], [SurveyId], [Text], [CreatedAt])
VALUES 
(1, 1, 'Option A', GETUTCDATE()),
(2, 1, 'Option B', GETUTCDATE());
SET IDENTITY_INSERT [dbo].[Options] OFF;

-- SEED: Insert votes (admin voted Option A, admin2 voted Option B)
SET IDENTITY_INSERT [dbo].[Votes] ON;
INSERT INTO [dbo].[Votes] ([Id], [UserId], [SurveyId], [OptionId], [CreatedAt])
VALUES 
(1, 1, 1, 1, GETUTCDATE()),
(2, 5, 1, 2, GETUTCDATE());
SET IDENTITY_INSERT [dbo].[Votes] OFF;