---------------------------------------------------------------------------------------
-- 1. CountryCityMaster
---------------------------------------------------------------------------------------
USE [HotelModule]
GO

/****** Object:  Table [dbo].[CountryCityMaster]    Script Date: 24-11-2025 12:46:48 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[CountryCityMaster](
	[MasterId] [int] IDENTITY(1,1) NOT NULL,
	[GroupName] [nvarchar](255) NOT NULL,
	[GroupValue] [nvarchar](max) NOT NULL,
	[HasChild] [int] NULL,
	[parentId] [int] NULL,
	[iso2] [nvarchar](255) NULL,
	[iso3] [nvarchar](255) NULL,
	[Code] [nvarchar](255) NULL,
	[statusFlag] [int] NULL,
	[CreatedBy] [nvarchar](255) NULL,
	[CreatedDate] [datetime] NULL,
	[UpdatedBy] [nvarchar](255) NULL,
	[UpdatedDate] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[MasterId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[CountryCityMaster] ADD  DEFAULT ((0)) FOR [HasChild]
GO

ALTER TABLE [dbo].[CountryCityMaster] ADD  DEFAULT ((0)) FOR [parentId]
GO

ALTER TABLE [dbo].[CountryCityMaster] ADD  DEFAULT ((0)) FOR [statusFlag]
GO

ALTER TABLE [dbo].[CountryCityMaster] ADD  DEFAULT ((0)) FOR [CreatedBy]
GO

ALTER TABLE [dbo].[CountryCityMaster] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[CountryCityMaster] ADD  DEFAULT ((0)) FOR [UpdatedBy]
GO

---------------------------------------------------------------------------------------
-- 1. CountryCityType
---------------------------------------------------------------------------------------
CREATE TYPE CountryCityType AS TABLE  
(
    SrNo INT,
    GroupName NVARCHAR(50),
    GroupValue NVARCHAR(200),
    HasChild BIT,
    ParentId INT,
    iso2 NVARCHAR(10),
    iso3 NVARCHAR(10)
);
---------------------------------------------------------------------------------------
-- 3. AddUpdateCountryCityData
---------------------------------------------------------------------------------------
ALTER PROCEDURE AddUpdateCountryCityData
(
    @countyCityData CountryCityType READONLY
)
AS
BEGIN
    SET NOCOUNT ON;

    MERGE INTO CountryCityMaster AS Target
    USING @countyCityData AS Source
        ON Target.GroupName = Source.GroupName 
       AND Target.GroupValue = Source.GroupValue  -- <-- Your unique match
    WHEN MATCHED THEN
        UPDATE SET 
            Target.HasChild = Source.HasChild,
            Target.ParentId = Source.ParentId,
            Target.iso2 = Source.iso2,
            Target.iso3 = Source.iso3,
			Target.UpdatedBy = 'SERVICE',
			Target.UpdatedDate = GETDATE()
    WHEN NOT MATCHED THEN
        INSERT (GroupName, GroupValue, HasChild, ParentId, iso2, iso3,CreatedBy)
        VALUES (Source.GroupName, Source.GroupValue, Source.HasChild, 
                Source.ParentId, Source.iso2, Source.iso3,'SERVICE')
    OUTPUT inserted.MasterId as Id;
END
---------------------------------------------------------------------------------------
-- 3. AddUpdateCountryCodeData
---------------------------------------------------------------------------------------

ALTER PROCEDURE AddUpdateCountryCodeData
(
    @CountryName NVARCHAR(200),
    @iso2 NVARCHAR(10),
    @Code NVARCHAR(20)
)
AS
BEGIN
    SET NOCOUNT ON;

    -- Check if this country already exists by country code
    IF EXISTS (SELECT 1 FROM CountryCityMaster WHERE GroupValue = @CountryName AND GroupName = 'country' AND iso2 = @iso2
	AND statusFlag = 0)
    BEGIN
        -- Update existing country
        UPDATE CountryCityMaster
        SET 
            Code = @Code,
			UpdatedBy = 'SERVICE-(COUNTRYCODE)',
			UpdatedDate = GETDATE()
        WHERE GroupValue = @CountryName
		AND GroupName = 'country'
		AND iso2 = @iso2

        -- Return updated Id
        SELECT MasterId FROM CountryCityMaster WHERE Code = @Code;
    END
END


---------------------------------------------------------------------------------------
--4. AGENT
---------------------------------------------------------------------------------------
USE [HotelModule]
GO

/****** Object:  Table [dbo].[AgentRegistration]    Script Date: 24-11-2025 01:47:48 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[AgentRegistration](
	[AgentId] [int] IDENTITY(1,1) NOT NULL,
	[AgentCode] [nvarchar](255) NOT NULL,
	[CompanyName] [nvarchar](255) NOT NULL,
	[WebsiteUrl] [nvarchar](255) NOT NULL,
	[RegistrationNumber] [nvarchar](100) NOT NULL,
	[LegalEntityName] [nvarchar](255) NOT NULL,
	[Country] [int] NOT NULL,
	[City] [int] NOT NULL,
	[Address] [nvarchar](500) NOT NULL,
	[PhoneCode] [nvarchar](10) NOT NULL,
	[PhoneNumber] [nvarchar](20) NOT NULL,
	[AdminFirstName] [nvarchar](100) NOT NULL,
	[AdminLastName] [nvarchar](100) NOT NULL,
	[Designation] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[AdminPhoneCode] [nvarchar](10) NOT NULL,
	[AdminPhoneNumber] [nvarchar](20) NOT NULL,
	[Currency] [nvarchar](10) NOT NULL,
	[Username] [nvarchar](100) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
	[FileName] [nvarchar](255) NULL,
	[FilePath] [nvarchar](255) NULL,
	[CreatedBy][nvarchar](255) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[UpdatedBy][nvarchar](255) NULL,
	[UpdatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AgentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[AgentRegistration] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
---------------------------------------------------------------------------------------
--AGENTSIGNUP
---------------------------------------------------------------------------------------


ALTER PROCEDURE AgentSignUp
(
    @CompanyName        NVARCHAR(255),
    @WebsiteUrl         NVARCHAR(255),
    @RegistrationNumber NVARCHAR(100),
    @LegalEntityName    NVARCHAR(255),
    @Country            INT,
    @City               INT,
    @Address            NVARCHAR(500),

    @PhoneCode          NVARCHAR(10),
    @PhoneNumber        NVARCHAR(20),

    @AdminFirstName     NVARCHAR(100),
    @AdminLastName      NVARCHAR(100),
    @Designation        NVARCHAR(100),

    @Email              NVARCHAR(255),
    @AdminPhoneCode     NVARCHAR(10),
    @AdminPhoneNumber   NVARCHAR(20),

    @Currency           NVARCHAR(10),
    @Username           NVARCHAR(100),
    @Password           NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AgentCode NVARCHAR(20);
    DECLARE @NewId INT;

    -- Generate AgentCode like AGT-123
    SET @AgentCode = 'AGT-' + RIGHT('000' + CAST((ABS(CHECKSUM(NEWID())) % 1000) AS VARCHAR(3)), 3);

    INSERT INTO AgentRegistration
    (
        AgentCode,
        CompanyName,
        WebsiteUrl,
        RegistrationNumber,
        LegalEntityName,
        Country,
        City,
        Address,
        PhoneCode,
        PhoneNumber,
        AdminFirstName,
        AdminLastName,
        Designation,
        Email,
        AdminPhoneCode,
        AdminPhoneNumber,
        Currency,
        Username,
        Password,
        CreatedDate,
        UpdatedDate
    )
    VALUES
    (
        @AgentCode,
        @CompanyName,
        @WebsiteUrl,
        @RegistrationNumber,
        @LegalEntityName,
        @Country,
        @City,
        @Address,
        @PhoneCode,
        @PhoneNumber,
        @AdminFirstName,
        @AdminLastName,
        @Designation,
        @Email,
        @AdminPhoneCode,
        @AdminPhoneNumber,
        @Currency,
        @Username,
        @Password,
        GETDATE(),
        GETDATE()
    );

    SET @NewId = SCOPE_IDENTITY();

    -- Return both ID & AgentCode
    SELECT 
        AgentId = @NewId,
        AgentCode = @AgentCode,
		status = 'success'	
END

---------------------------------------------------------------------------------------
--UpdateAgentDetails
---------------------------------------------------------------------------------------
CREATE PROCEDURE UpdateAgentDetails
(
    @agentId    INT,
    @agentCode  NVARCHAR(255),
    @fileName   NVARCHAR(255),
    @filePath   NVARCHAR(255)
)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Message NVARCHAR(255);

    -- Update table
    UPDATE AgentRegistration
    SET 
        FileName = @fileName,
        FilePath = @filePath,
        UpdatedDate = GETDATE()
    WHERE AgentId = @agentId AND 
        AgentCode = @agentCode

    -- Set message
    IF @@ROWCOUNT > 0
        SET @Message = 'Agent details updated successfully.';
    ELSE
        SET @Message = 'No record updated. Invalid AgentId.';

    -- Return updated data + message
    SELECT 
        AgentId,
        AgentCode,
        @Message AS Message
		FROM AgentRegistration WHERE AgentId = @AgentId;
END
GO
---------------------------------------------------------------------------------------