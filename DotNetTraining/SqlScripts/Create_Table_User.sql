CREATE TABLE "users"
(
    "Id" NVARCHAR(MAX) NOT NULL,
    "Name" NVARCHAR(MAX),
    "Email" NVARCHAR(MAX),
    "Password" NVARCHAR(MAX),
    "CreatedAt" NVARCHAR(MAX),
    "UpdatedAt" NVARCHAR(MAX),
    "Id" NVARCHAR(MAX),
    "Role" NVARCHAR(50) DEFAULT 'user',

    CONSTRAINT "users_pkey" PRIMARY KEY ("Id")
)

ALTER TABLE users ADD Role NVARCHAR(MAX) NULL;
