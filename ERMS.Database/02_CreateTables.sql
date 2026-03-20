USE `risk_management_db`;

-- 1. User_Master
CREATE TABLE IF NOT EXISTS `User_Master` (
    `UserId`          INT AUTO_INCREMENT PRIMARY KEY,
    `Username`        VARCHAR(100) NOT NULL UNIQUE,
    `LoginType`       VARCHAR(10)  NOT NULL,
    `Password`        VARCHAR(200),
    `FullName`        VARCHAR(100) NOT NULL,
    `Email`           VARCHAR(200) NOT NULL,
    `Mobile`          VARCHAR(20),
    `Status`          VARCHAR(10)  DEFAULT 'Active',
    `AdminFlag`       VARCHAR(10)  DEFAULT 'N',
    `FailedAttempts`  INT          DEFAULT 0,
    `IsLocked`        TINYINT(1)   DEFAULT 0,
    `LastFailedLogin` DATETIME     NULL,
    `CreatedBy`       INT,
    `CreatedDate`     DATETIME     DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`       INT,
    `UpdatedDate`     DATETIME     DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 2. FY_Master
CREATE TABLE IF NOT EXISTS `FY_Master` (
    `FYId`        VARCHAR(20) PRIMARY KEY,
    `FYName`      VARCHAR(100),
    `FromDate`    DATETIME,
    `ToDate`      DATETIME,
    `CreatedBy`   INT,
    `CreatedDate` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`   INT,
    `UpdatedDate` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 3. BU_Master
CREATE TABLE IF NOT EXISTS `BU_Master` (
    `BUId`        VARCHAR(10) PRIMARY KEY,
    `BUName`      VARCHAR(200),
    `BUShortName` VARCHAR(50),
    `Status`      VARCHAR(10)  DEFAULT 'Active',
    `CreatedBy`   INT,
    `CreatedDate` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`   INT,
    `UpdatedDate` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 4. RiskCategory_Master
CREATE TABLE IF NOT EXISTS `RiskCategory_Master` (
    `RiskCatId`    INT AUTO_INCREMENT PRIMARY KEY,
    `RiskCatName`  VARCHAR(100),
    `RiskCatAlias` VARCHAR(100),
    `Status`       VARCHAR(10)  DEFAULT 'Active',
    `CreatedBy`    INT,
    `CreatedDate`  DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`    INT,
    `UpdatedDate`  DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 5. RiskSubCategory_Master
CREATE TABLE IF NOT EXISTS `RiskSubCategory_Master` (
    `RiskSubCatId`   INT AUTO_INCREMENT PRIMARY KEY,
    `RiskCatId`      INT NOT NULL,
    `RiskSubCatName` VARCHAR(100),
    `RiskCatAlias`   VARCHAR(100),
    `Status`         VARCHAR(10)  DEFAULT 'Active',
    `CreatedBy`      INT,
    `CreatedDate`    DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`      INT,
    `UpdatedDate`    DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    CONSTRAINT `FK_RiskSubCategory_Category`
        FOREIGN KEY (`RiskCatId`)
        REFERENCES `RiskCategory_Master`(`RiskCatId`)
        ON DELETE CASCADE
) ENGINE=InnoDB;

-- 6. Function_Master
CREATE TABLE IF NOT EXISTS `Function_Master` (
    `FunctionId`   INT AUTO_INCREMENT PRIMARY KEY,
    `FunctionName` VARCHAR(200),
    `Status`       VARCHAR(10)  DEFAULT 'Active',
    `CreatedBy`    INT,
    `CreatedDate`  DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`    INT,
    `UpdatedDate`  DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 7. ActionFrequency_Master
CREATE TABLE IF NOT EXISTS `ActionFrequency_Master` (
    `AFId`        VARCHAR(50) PRIMARY KEY,
    `AFName`      VARCHAR(100),
    `Days`        INT,
    `Status`      VARCHAR(10)  DEFAULT 'Active',
    `CreatedBy`   INT,
    `CreatedDate` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`   INT,
    `UpdatedDate` DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 8. ActionStatus_Master
CREATE TABLE IF NOT EXISTS `ActionStatus_Master` (
    `StatusId`    VARCHAR(10) PRIMARY KEY,
    `StatusName`  VARCHAR(50),
    `DisplayFlag` VARCHAR(10) DEFAULT 'Y'
) ENGINE=InnoDB;

-- 9. LoginAudit_Log
CREATE TABLE IF NOT EXISTS `LoginAudit_Log` (
    `LogId`         BIGINT AUTO_INCREMENT PRIMARY KEY,
    `UserId`        INT NULL,
    `Username`      VARCHAR(100),
    `LoginType`     VARCHAR(20),
    `Status`        VARCHAR(20),
    `IPAddress`     VARCHAR(50),
    `FailureReason` VARCHAR(255),
    `CreatedDate`   DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 10. UserPermission
CREATE TABLE IF NOT EXISTS `UserPermission` (
    `PermissionId` INT AUTO_INCREMENT PRIMARY KEY,
    `UserId`       INT         NOT NULL,
    `BUId`         VARCHAR(10) NOT NULL,
    `Role`         CHAR(1)     NOT NULL,
    `CreatedBy`    INT,
    `CreatedDate`  DATETIME DEFAULT CURRENT_TIMESTAMP,
    CONSTRAINT `FK_UserPermission_User`
        FOREIGN KEY (`UserId`) REFERENCES `User_Master`(`UserId`) ON DELETE CASCADE,
    CONSTRAINT `FK_UserPermission_BU`
        FOREIGN KEY (`BUId`)   REFERENCES `BU_Master`(`BUId`)    ON DELETE CASCADE,
    CONSTRAINT `CHK_UserPermission_Role`
        CHECK (`Role` IN ('O','C','V','N')),
    UNIQUE KEY `UK_User_BU` (`UserId`, `BUId`)
) ENGINE=InnoDB;

-- 11. UserPermission_Log
CREATE TABLE IF NOT EXISTS `UserPermission_Log` (
    `LogId`        BIGINT AUTO_INCREMENT PRIMARY KEY,
    `PermissionId` INT,
    `UserId`       INT,
    `BUId`         VARCHAR(10),
    `OldRole`      CHAR(1),
    `NewRole`      CHAR(1),
    `ActionType`   VARCHAR(10),
    `ChangedBy`    INT,
    `ChangedDate`  DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB;

-- 12. Risk_Master
CREATE TABLE IF NOT EXISTS `Risk_Master` (
    `RiskId`       INT AUTO_INCREMENT PRIMARY KEY,
    `BUId`         VARCHAR(10) NOT NULL,
    `FYId`         VARCHAR(20) NOT NULL,
    `RiskCatId`    INT         NOT NULL,
    `RiskSubCatId` INT         NOT NULL,
    `FunctionId`   INT         NOT NULL,
    `RiskTitle`    VARCHAR(300) NOT NULL,
    `Description`  TEXT        NOT NULL,
    `ImpactLevel`  VARCHAR(10) NOT NULL,
    `Likelihood`   VARCHAR(10) NOT NULL,
    `Status`       VARCHAR(30) DEFAULT 'Draft',
    `Remarks`      TEXT,
    `ReviewedBy`   INT NULL,
    `ReviewedDate` DATETIME NULL,
    `CreatedBy`    INT NOT NULL,
    `CreatedDate`  DATETIME DEFAULT CURRENT_TIMESTAMP,
    `UpdatedBy`    INT,
    `UpdatedDate`  DATETIME DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (`BUId`)         REFERENCES `BU_Master`(`BUId`),
    FOREIGN KEY (`FYId`)         REFERENCES `FY_Master`(`FYId`),
    FOREIGN KEY (`RiskCatId`)    REFERENCES `RiskCategory_Master`(`RiskCatId`),
    FOREIGN KEY (`RiskSubCatId`) REFERENCES `RiskSubCategory_Master`(`RiskSubCatId`),
    FOREIGN KEY (`FunctionId`)   REFERENCES `Function_Master`(`FunctionId`),
    FOREIGN KEY (`CreatedBy`)    REFERENCES `User_Master`(`UserId`)
) ENGINE=InnoDB;

-- 13. Risk_History
CREATE TABLE IF NOT EXISTS `Risk_History` (
    `HistoryId`   INT AUTO_INCREMENT PRIMARY KEY,
    `RiskId`      INT NOT NULL,
    `OldStatus`   VARCHAR(30),
    `NewStatus`   VARCHAR(30),
    `Action`      VARCHAR(50),
    `Remarks`     TEXT,
    `ChangedBy`   INT NOT NULL,
    `ChangedDate` DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (`RiskId`)    REFERENCES `Risk_Master`(`RiskId`),
    FOREIGN KEY (`ChangedBy`) REFERENCES `User_Master`(`UserId`)
) ENGINE=InnoDB;

-- 14. User_Audit_Log
CREATE TABLE IF NOT EXISTS `User_Audit_Log` (
    `AuditId`     INT AUTO_INCREMENT PRIMARY KEY,
    `UserId`      INT,
    `ActionType`  VARCHAR(20),
    `ChangedBy`   INT,
    `ChangedDate` DATETIME DEFAULT CURRENT_TIMESTAMP,
    `OldData`     TEXT,
    `NewData`     TEXT
) ENGINE=InnoDB;

-- 15. BU_Audit_Log
CREATE TABLE IF NOT EXISTS `BU_Audit_Log` (
    `AuditId`       BIGINT AUTO_INCREMENT PRIMARY KEY,
    `BUId`          VARCHAR(10),
    `ActionType`    VARCHAR(20),
    `ChangedBy`     INT,
    `ChangedDate`   DATETIME DEFAULT CURRENT_TIMESTAMP,
    `ChangeSummary` TEXT
) ENGINE=InnoDB;

-- 16. RiskCategory_Audit_Log
CREATE TABLE IF NOT EXISTS `RiskCategory_Audit_Log` (
    `AuditId`       BIGINT AUTO_INCREMENT PRIMARY KEY,
    `RiskCatId`     INT,
    `ActionType`    VARCHAR(20),
    `ChangedBy`     INT,
    `ChangedDate`   DATETIME DEFAULT CURRENT_TIMESTAMP,
    `ChangeSummary` TEXT
) ENGINE=InnoDB;

-- 17. RiskSubCategory_Audit_Log
CREATE TABLE IF NOT EXISTS `RiskSubCategory_Audit_Log` (
    `AuditId`       BIGINT AUTO_INCREMENT PRIMARY KEY,
    `RiskSubCatId`  INT,
    `ActionType`    VARCHAR(20),
    `ChangedBy`     INT,
    `ChangedDate`   DATETIME DEFAULT CURRENT_TIMESTAMP,
    `ChangeSummary` TEXT
) ENGINE=InnoDB;

-- 18. Function_Audit_Log
CREATE TABLE IF NOT EXISTS `Function_Audit_Log` (
    `AuditId`       BIGINT AUTO_INCREMENT PRIMARY KEY,
    `FunctionId`    INT,
    `ActionType`    VARCHAR(20),
    `ChangedBy`     INT,
    `ChangedDate`   DATETIME DEFAULT CURRENT_TIMESTAMP,
    `ChangeSummary` TEXT
) ENGINE=InnoDB;
