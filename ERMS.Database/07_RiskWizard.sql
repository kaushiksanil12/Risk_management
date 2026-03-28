-- ============================================================
-- ERMS Risk Wizard Extension — Migration Script
-- Run once on risk_management_db
-- ============================================================

USE risk_management_db;

-- ─────────────────────────────────────────────────────────────
-- TABLE 1: Risk_PeopleOverview  (Step 2)
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `Risk_PeopleOverview` (
    `Id`          INT AUTO_INCREMENT PRIMARY KEY,
    `RiskId`      INT NOT NULL UNIQUE,
    `OwnerId`     INT NOT NULL,
    `ChampionId`  INT NOT NULL,
    `CreatedBy`   INT,
    `CreatedDate` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`   INT,
    `UpdatedDate` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`RiskId`)     REFERENCES `Risk_Master`(`RiskId`) ON DELETE CASCADE,
    FOREIGN KEY (`OwnerId`)    REFERENCES `User_Master`(`UserId`),
    FOREIGN KEY (`ChampionId`) REFERENCES `User_Master`(`UserId`)
) ENGINE=InnoDB;

-- ─────────────────────────────────────────────────────────────
-- TABLE 2: Risk_Assessment  (Step 3)
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `Risk_Assessment` (
    `Id`              INT AUTO_INCREMENT PRIMARY KEY,
    `RiskId`          INT NOT NULL UNIQUE,
    `ResponseMeasure` TEXT NULL,
    `CurrentControls` TEXT NULL,
    `LineOfAction`    TEXT NULL,
    `ResponsibleTeam` VARCHAR(200) NULL,
    `TargetDate`      DATE NULL,
    `FrequencyId`     VARCHAR(50) NULL,
    `CreatedBy`       INT,
    `CreatedDate`     DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`       INT,
    `UpdatedDate`     DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`RiskId`)      REFERENCES `Risk_Master`(`RiskId`) ON DELETE CASCADE,
    FOREIGN KEY (`FrequencyId`) REFERENCES `ActionFrequency_Master`(`AFId`)
) ENGINE=InnoDB;

-- ─────────────────────────────────────────────────────────────
-- TABLE 3: Risk_QuarterRating  (Step 4)
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `Risk_QuarterRating` (
    `Id`                 INT AUTO_INCREMENT PRIMARY KEY,
    `RiskId`             INT NOT NULL,
    `Quarter`            VARCHAR(5) NOT NULL,
    `GrossImpact`        VARCHAR(30) NULL,
    `GrossLikelihood`    VARCHAR(30) NULL,
    `GrossScore`         INT NULL,
    `GrossRating`        VARCHAR(50) NULL,
    `ResidualImpact`     VARCHAR(30) NULL,
    `ResidualLikelihood` VARCHAR(30) NULL,
    `ResidualScore`      INT NULL,
    `ResidualRating`     VARCHAR(50) NULL,
    `CreatedBy`          INT,
    `CreatedDate`        DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`          INT,
    `UpdatedDate`        DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`RiskId`) REFERENCES `Risk_Master`(`RiskId`) ON DELETE CASCADE,
    UNIQUE KEY `UK_Risk_Quarter` (`RiskId`, `Quarter`)
) ENGINE=InnoDB;

-- ─────────────────────────────────────────────────────────────
-- TABLE 4: Risk_Attachments  (Step 5)
-- ─────────────────────────────────────────────────────────────
CREATE TABLE IF NOT EXISTS `Risk_Attachments` (
    `Id`           INT AUTO_INCREMENT PRIMARY KEY,
    `RiskId`       INT NOT NULL,
    `FileName`     VARCHAR(255) NOT NULL,
    `FilePath`     VARCHAR(500) NOT NULL,
    `FileSize`     INT NULL,
    `FileType`     VARCHAR(100) NULL,
    `UploadedBy`   INT,
    `UploadedDate` DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (`RiskId`)     REFERENCES `Risk_Master`(`RiskId`) ON DELETE CASCADE,
    FOREIGN KEY (`UploadedBy`) REFERENCES `User_Master`(`UserId`)
) ENGINE=InnoDB;

-- ─────────────────────────────────────────────────────────────
-- STORED PROCEDURES
-- ─────────────────────────────────────────────────────────────

DROP PROCEDURE IF EXISTS sp_RiskPeople_InsertOrUpdate;
DELIMITER $$
CREATE PROCEDURE sp_RiskPeople_InsertOrUpdate(
    IN p_RiskId     INT,
    IN p_OwnerId    INT,
    IN p_ChampionId INT,
    IN p_UserId     INT
)
BEGIN
    IF EXISTS (SELECT 1 FROM Risk_PeopleOverview WHERE RiskId = p_RiskId) THEN
        UPDATE Risk_PeopleOverview
        SET OwnerId     = p_OwnerId,
            ChampionId  = p_ChampionId,
            UpdatedBy   = p_UserId,
            UpdatedDate = NOW()
        WHERE RiskId = p_RiskId;
    ELSE
        INSERT INTO Risk_PeopleOverview(RiskId, OwnerId, ChampionId, CreatedBy, CreatedDate)
        VALUES(p_RiskId, p_OwnerId, p_ChampionId, p_UserId, NOW());
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_RiskPeople_GetByRisk;
DELIMITER $$
CREATE PROCEDURE sp_RiskPeople_GetByRisk(IN p_RiskId INT)
BEGIN
    SELECT
        rp.Id,
        rp.RiskId,
        rp.OwnerId,
        o.FullName  AS OwnerName,
        o.Username  AS OwnerUsername,
        rp.ChampionId,
        c.FullName  AS ChampionName,
        c.Username  AS ChampionUsername
    FROM Risk_PeopleOverview rp
    LEFT JOIN User_Master o ON rp.OwnerId    = o.UserId
    LEFT JOIN User_Master c ON rp.ChampionId = c.UserId
    WHERE rp.RiskId = p_RiskId;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_RiskAssessment_InsertOrUpdate;
DELIMITER $$
CREATE PROCEDURE sp_RiskAssessment_InsertOrUpdate(
    IN p_RiskId          INT,
    IN p_ResponseMeasure TEXT,
    IN p_CurrentControls TEXT,
    IN p_LineOfAction    TEXT,
    IN p_ResponsibleTeam VARCHAR(200),
    IN p_TargetDate      DATE,
    IN p_FrequencyId     VARCHAR(50),
    IN p_UserId          INT
)
BEGIN
    IF EXISTS (SELECT 1 FROM Risk_Assessment WHERE RiskId = p_RiskId) THEN
        UPDATE Risk_Assessment
        SET ResponseMeasure = p_ResponseMeasure,
            CurrentControls = p_CurrentControls,
            LineOfAction    = p_LineOfAction,
            ResponsibleTeam = p_ResponsibleTeam,
            TargetDate      = p_TargetDate,
            FrequencyId     = p_FrequencyId,
            UpdatedBy       = p_UserId,
            UpdatedDate     = NOW()
        WHERE RiskId = p_RiskId;
    ELSE
        INSERT INTO Risk_Assessment(
            RiskId, ResponseMeasure, CurrentControls, LineOfAction,
            ResponsibleTeam, TargetDate, FrequencyId, CreatedBy, CreatedDate)
        VALUES(
            p_RiskId, p_ResponseMeasure, p_CurrentControls, p_LineOfAction,
            p_ResponsibleTeam, p_TargetDate, p_FrequencyId, p_UserId, NOW());
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_RiskAssessment_GetByRisk;
DELIMITER $$
CREATE PROCEDURE sp_RiskAssessment_GetByRisk(IN p_RiskId INT)
BEGIN
    SELECT
        ra.Id,
        ra.RiskId,
        ra.ResponseMeasure,
        ra.CurrentControls,
        ra.LineOfAction,
        ra.ResponsibleTeam,
        ra.TargetDate,
        ra.FrequencyId,
        af.AFName AS FrequencyName
    FROM Risk_Assessment ra
    LEFT JOIN ActionFrequency_Master af ON ra.FrequencyId = af.AFId
    WHERE ra.RiskId = p_RiskId;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_RiskQuarterRating_InsertOrUpdate;
DELIMITER $$
CREATE PROCEDURE sp_RiskQuarterRating_InsertOrUpdate(
    IN p_RiskId             INT,
    IN p_Quarter            VARCHAR(5),
    IN p_GrossImpact        VARCHAR(30),
    IN p_GrossLikelihood    VARCHAR(30),
    IN p_GrossScore         INT,
    IN p_GrossRating        VARCHAR(50),
    IN p_ResidualImpact     VARCHAR(30),
    IN p_ResidualLikelihood VARCHAR(30),
    IN p_ResidualScore      INT,
    IN p_ResidualRating     VARCHAR(50),
    IN p_UserId             INT
)
BEGIN
    IF EXISTS (
        SELECT 1 FROM Risk_QuarterRating
        WHERE RiskId = p_RiskId AND Quarter = p_Quarter
    ) THEN
        UPDATE Risk_QuarterRating
        SET GrossImpact        = p_GrossImpact,
            GrossLikelihood    = p_GrossLikelihood,
            GrossScore         = p_GrossScore,
            GrossRating        = p_GrossRating,
            ResidualImpact     = p_ResidualImpact,
            ResidualLikelihood = p_ResidualLikelihood,
            ResidualScore      = p_ResidualScore,
            ResidualRating     = p_ResidualRating,
            UpdatedBy          = p_UserId,
            UpdatedDate        = NOW()
        WHERE RiskId = p_RiskId AND Quarter = p_Quarter;
    ELSE
        INSERT INTO Risk_QuarterRating(
            RiskId, Quarter,
            GrossImpact, GrossLikelihood, GrossScore, GrossRating,
            ResidualImpact, ResidualLikelihood, ResidualScore, ResidualRating,
            CreatedBy, CreatedDate)
        VALUES(
            p_RiskId, p_Quarter,
            p_GrossImpact, p_GrossLikelihood, p_GrossScore, p_GrossRating,
            p_ResidualImpact, p_ResidualLikelihood, p_ResidualScore, p_ResidualRating,
            p_UserId, NOW());
    END IF;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_RiskQuarterRating_GetByRisk;
DELIMITER $$
CREATE PROCEDURE sp_RiskQuarterRating_GetByRisk(IN p_RiskId INT)
BEGIN
    SELECT Id, RiskId, Quarter,
           GrossImpact, GrossLikelihood, GrossScore, GrossRating,
           ResidualImpact, ResidualLikelihood, ResidualScore, ResidualRating,
           CreatedDate, UpdatedDate
    FROM Risk_QuarterRating
    WHERE RiskId = p_RiskId
    ORDER BY Quarter;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_RiskAttachment_Insert;
DELIMITER $$
CREATE PROCEDURE sp_RiskAttachment_Insert(
    IN p_RiskId    INT,
    IN p_FileName  VARCHAR(255),
    IN p_FilePath  VARCHAR(500),
    IN p_FileSize  INT,
    IN p_FileType  VARCHAR(100),
    IN p_UserId    INT
)
BEGIN
    INSERT INTO Risk_Attachments(
        RiskId, FileName, FilePath, FileSize, FileType, UploadedBy, UploadedDate)
    VALUES(
        p_RiskId, p_FileName, p_FilePath, p_FileSize, p_FileType, p_UserId, NOW());
    SELECT LAST_INSERT_ID() AS NewId;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_RiskAttachment_GetByRisk;
DELIMITER $$
CREATE PROCEDURE sp_RiskAttachment_GetByRisk(IN p_RiskId INT)
BEGIN
    SELECT
        a.Id, a.RiskId, a.FileName, a.FilePath,
        a.FileSize, a.FileType, a.UploadedDate,
        u.FullName AS UploadedByName
    FROM Risk_Attachments a
    LEFT JOIN User_Master u ON a.UploadedBy = u.UserId
    WHERE a.RiskId = p_RiskId
    ORDER BY a.UploadedDate DESC;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_RiskAttachment_Delete;
DELIMITER $$
CREATE PROCEDURE sp_RiskAttachment_Delete(IN p_AttachmentId INT, IN p_RiskId INT)
BEGIN
    DELETE FROM Risk_Attachments
    WHERE Id = p_AttachmentId AND RiskId = p_RiskId;
END$$
DELIMITER ;

DROP PROCEDURE IF EXISTS sp_RiskFull_GetById;
DELIMITER $$
CREATE PROCEDURE sp_RiskFull_GetById(IN p_RiskId INT)
BEGIN
    -- Result set 1: Core risk
    SELECT r.*,
           b.BUName, f.FYName,
           rc.RiskCatName, rs.RiskSubCatName,
           fn.FunctionName,
           u.FullName AS CreatedByName
    FROM Risk_Master r
    LEFT JOIN BU_Master              b  ON r.BUId         = b.BUId
    LEFT JOIN FY_Master              f  ON r.FYId         = f.FYId
    LEFT JOIN RiskCategory_Master    rc ON r.RiskCatId    = rc.RiskCatId
    LEFT JOIN RiskSubCategory_Master rs ON r.RiskSubCatId = rs.RiskSubCatId
    LEFT JOIN Function_Master        fn ON r.FunctionId   = fn.FunctionId
    LEFT JOIN User_Master            u  ON r.CreatedBy    = u.UserId
    WHERE r.RiskId = p_RiskId;

    -- Result set 2: People Overview
    SELECT rp.*, o.FullName AS OwnerName, c.FullName AS ChampionName
    FROM Risk_PeopleOverview rp
    LEFT JOIN User_Master o ON rp.OwnerId    = o.UserId
    LEFT JOIN User_Master c ON rp.ChampionId = c.UserId
    WHERE rp.RiskId = p_RiskId;

    -- Result set 3: Assessment
    SELECT ra.*, af.AFName AS FrequencyName
    FROM Risk_Assessment ra
    LEFT JOIN ActionFrequency_Master af ON ra.FrequencyId = af.AFId
    WHERE ra.RiskId = p_RiskId;

    -- Result set 4: Quarter Ratings
    SELECT * FROM Risk_QuarterRating
    WHERE RiskId = p_RiskId ORDER BY Quarter;

    -- Result set 5: Attachments
    SELECT a.*, u.FullName AS UploadedByName
    FROM Risk_Attachments a
    LEFT JOIN User_Master u ON a.UploadedBy = u.UserId
    WHERE a.RiskId = p_RiskId
    ORDER BY a.UploadedDate DESC;

    -- Result set 6: History
    SELECT h.*, u.FullName AS ChangedByName
    FROM Risk_History h
    LEFT JOIN User_Master u ON h.ChangedBy = u.UserId
    WHERE h.RiskId = p_RiskId
    ORDER BY h.ChangedDate DESC;
END$$
DELIMITER ;
