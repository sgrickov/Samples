IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    CREATE TABLE [Locations] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(450) NOT NULL,
        [IsActive] bit NOT NULL,
        CONSTRAINT [PK_Locations] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    CREATE TABLE [SearchRuns] (
        [Id] int NOT NULL IDENTITY,
        [StartedAtUtc] datetime2 NOT NULL,
        [CompletedAtUtc] datetime2 NULL,
        CONSTRAINT [PK_SearchRuns] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    CREATE TABLE [SearchRunLocations] (
        [Id] int NOT NULL IDENTITY,
        [SearchRunId] int NOT NULL,
        [LocationName] nvarchar(max) NOT NULL,
        [ResultCount] int NOT NULL,
        CONSTRAINT [PK_SearchRunLocations] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SearchRunLocations_SearchRuns_SearchRunId] FOREIGN KEY ([SearchRunId]) REFERENCES [SearchRuns] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    CREATE TABLE [SolicitorListings] (
        [Id] int NOT NULL IDENTITY,
        [SearchRunLocationId] int NOT NULL,
        [Name] nvarchar(max) NOT NULL,
        [Address] nvarchar(max) NOT NULL,
        [Phone] nvarchar(max) NULL,
        [Rating] decimal(3,2) NOT NULL,
        [ReviewCount] int NOT NULL,
        [ProfileUrl] nvarchar(max) NOT NULL,
        [WebsiteUrl] nvarchar(max) NULL,
        [EmailFormUrl] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [IsBasicListing] bit NOT NULL,
        CONSTRAINT [PK_SolicitorListings] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SolicitorListings_SearchRunLocations_SearchRunLocationId] FOREIGN KEY ([SearchRunLocationId]) REFERENCES [SearchRunLocations] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    CREATE TABLE [SolicitorQualityMarks] (
        [Id] int NOT NULL IDENTITY,
        [SolicitorListingId] int NOT NULL,
        [MarkName] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_SolicitorQualityMarks] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SolicitorQualityMarks_SolicitorListings_SolicitorListingId] FOREIGN KEY ([SolicitorListingId]) REFERENCES [SolicitorListings] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Locations]'))
        SET IDENTITY_INSERT [Locations] ON;
    EXEC(N'INSERT INTO [Locations] ([Id], [IsActive], [Name])
    VALUES (1, CAST(1 AS bit), N''London''),
    (2, CAST(1 AS bit), N''Birmingham''),
    (3, CAST(1 AS bit), N''Leeds''),
    (4, CAST(1 AS bit), N''Manchester''),
    (5, CAST(1 AS bit), N''Sheffield''),
    (6, CAST(1 AS bit), N''Bradford''),
    (7, CAST(1 AS bit), N''Liverpool''),
    (8, CAST(1 AS bit), N''Bristol'')');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'IsActive', N'Name') AND [object_id] = OBJECT_ID(N'[Locations]'))
        SET IDENTITY_INSERT [Locations] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Locations_Name] ON [Locations] ([Name]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SearchRunLocations_SearchRunId] ON [SearchRunLocations] ([SearchRunId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SolicitorListings_SearchRunLocationId] ON [SolicitorListings] ([SearchRunLocationId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    CREATE INDEX [IX_SolicitorQualityMarks_SolicitorListingId] ON [SolicitorQualityMarks] ([SolicitorListingId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718094309_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260718094309_InitialCreate', N'10.0.10');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718100445_AddIsNewSinceLastRun'
)
BEGIN
    ALTER TABLE [SolicitorListings] ADD [IsNewSinceLastRun] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718100445_AddIsNewSinceLastRun'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260718100445_AddIsNewSinceLastRun', N'10.0.10');
END;

COMMIT;
GO

