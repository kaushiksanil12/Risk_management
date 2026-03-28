USE `risk_management_db`;

-- ============================================================
-- AUTHENTICATION
-- ============================================================

DELIMITER $$
CREATE PROCEDURE `sp_GetUserForLogin`(IN p_Username VARCHAR(100))
BEGIN
    SELECT `UserId`, `Password`, `LoginType`, `Status`, `FailedAttempts`, `IsLocked`
    FROM `User_Master`
    WHERE `Username` = p_Username
    LIMIT 1;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `sp_UpdateFailedAttempts`(IN p_UserId INT, IN p_Attempts INT)
BEGIN
    UPDATE `User_Master`
    SET `FailedAttempts` = p_Attempts, `LastFailedLogin` = NOW()
    WHERE `UserId` = p_UserId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `sp_ResetFailedAttempts`(IN p_UserId INT)
BEGIN
    UPDATE `User_Master`
    SET `FailedAttempts` = 0, `IsLocked` = 0
    WHERE `UserId` = p_UserId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `sp_LockAccount`(IN p_UserId INT)
BEGIN
    UPDATE `User_Master` SET `IsLocked` = 1 WHERE `UserId` = p_UserId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE `sp_InsertLoginAudit`(
    IN p_UserId INT, IN p_Username VARCHAR(100), IN p_LoginType VARCHAR(20),
    IN p_Status VARCHAR(20), IN p_IPAddress VARCHAR(50), IN p_FailureReason VARCHAR(255))
BEGIN
    INSERT INTO `LoginAudit_Log`(`UserId`,`Username`,`LoginType`,`Status`,`IPAddress`,`FailureReason`)
    VALUES (p_UserId, p_Username, p_LoginType, p_Status, p_IPAddress, p_FailureReason);
END$$
DELIMITER ;

-- ============================================================
-- USER MANAGEMENT
-- ============================================================

DELIMITER $$
CREATE PROCEDURE sp_User_Search(IN p_Search VARCHAR(100), IN p_Status VARCHAR(10))
BEGIN
    SELECT UserId, Username, FullName, Email, LoginType, `Status`, IsLocked, AdminFlag
    FROM User_Master
    WHERE (p_Search IS NULL OR p_Search = ''
           OR Username LIKE CONCAT('%', p_Search, '%')
           OR FullName  LIKE CONCAT('%', p_Search, '%'))
      AND (p_Status IS NULL OR p_Status = '' OR `Status` = p_Status)
    ORDER BY Username;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_User_GetById(IN p_UserId INT)
BEGIN
    SELECT UserId, Username, LoginType, FullName, Email, Mobile, Status, AdminFlag
    FROM User_Master
    WHERE UserId = p_UserId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_User_Insert(
    IN p_Username VARCHAR(100), IN p_LoginType VARCHAR(20), IN p_Password VARCHAR(200),
    IN p_FullName VARCHAR(100), IN p_Email VARCHAR(200), IN p_Mobile VARCHAR(20),
    IN p_Status VARCHAR(10), IN p_AdminFlag VARCHAR(1), IN p_CreatedBy INT)
BEGIN
    INSERT INTO User_Master(Username, LoginType, Password, FullName, Email, Mobile,
        Status, AdminFlag, CreatedBy, CreatedDate)
    VALUES(p_Username, p_LoginType, p_Password, p_FullName, p_Email, p_Mobile,
        p_Status, p_AdminFlag, p_CreatedBy, NOW());
    SELECT LAST_INSERT_ID() AS NewId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_User_Update(
    IN p_UserId INT, IN p_LoginType VARCHAR(20), IN p_Password VARCHAR(200),
    IN p_FullName VARCHAR(100), IN p_Email VARCHAR(200), IN p_Mobile VARCHAR(20),
    IN p_Status VARCHAR(10), IN p_AdminFlag VARCHAR(1), IN p_UpdatedBy INT)
BEGIN
    UPDATE User_Master
    SET LoginType  = p_LoginType,
        Password   = CASE WHEN p_Password IS NULL OR p_Password = '' THEN Password ELSE p_Password END,
        FullName   = p_FullName,
        Email      = p_Email,
        Mobile     = p_Mobile,
        Status     = p_Status,
        AdminFlag  = p_AdminFlag,
        UpdatedBy  = p_UpdatedBy,
        UpdatedDate = NOW()
    WHERE UserId = p_UserId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_User_CheckUsername(IN p_Username VARCHAR(100), IN p_UserId INT)
BEGIN
    SELECT COUNT(*) AS Cnt FROM User_Master
    WHERE Username = p_Username AND (p_UserId IS NULL OR UserId <> p_UserId);
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_User_Unlock(IN p_UserId INT, IN p_UpdatedBy INT)
BEGIN
    UPDATE User_Master
    SET FailedAttempts = 0, IsLocked = 0, UpdatedBy = p_UpdatedBy, UpdatedDate = NOW()
    WHERE UserId = p_UserId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_User_Dropdown()
BEGIN
    SELECT UserId, CONCAT(Username, ' (', FullName, ')') AS DisplayName
    FROM User_Master WHERE Status = 'Active' ORDER BY Username;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_User_Audit_Insert(
    IN p_UserId INT, IN p_ActionType VARCHAR(20), IN p_ChangedBy INT,
    IN p_OldData TEXT, IN p_NewData TEXT)
BEGIN
    INSERT INTO User_Audit_Log(UserId, ActionType, ChangedBy, ChangedDate, OldData, NewData)
    VALUES(p_UserId, p_ActionType, p_ChangedBy, NOW(), p_OldData, p_NewData);
END$$
DELIMITER ;

-- ============================================================
-- BUSINESS UNIT (BU) MANAGEMENT
-- ============================================================

DELIMITER $$
CREATE PROCEDURE sp_BU_Search(IN p_Search VARCHAR(200), IN p_Status VARCHAR(10))
BEGIN
    SELECT BUId, BUName, BUShortName, Status FROM BU_Master
    WHERE (p_Search IS NULL OR p_Search = ''
           OR BUName LIKE CONCAT('%', p_Search, '%')
           OR BUShortName LIKE CONCAT('%', p_Search, '%'))
      AND (p_Status IS NULL OR p_Status = '' OR Status = p_Status)
    ORDER BY BUName;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_BU_GetById(IN p_BUId VARCHAR(10))
BEGIN
    SELECT BUId, BUName, BUShortName, Status FROM BU_Master WHERE BUId = p_BUId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_BU_Insert(
    IN p_BUName VARCHAR(200), IN p_BUShortName VARCHAR(50),
    IN p_Status VARCHAR(10), IN p_CreatedBy INT)
BEGIN
    DECLARE v_MaxId INT DEFAULT 0;
    DECLARE v_NewId VARCHAR(10);
    -- LOCK TABLES BU_Master WRITE;
    SELECT IFNULL(MAX(CAST(SUBSTRING(BUId, 3) AS UNSIGNED)), 0) INTO v_MaxId FROM BU_Master;
    SET v_MaxId = v_MaxId + 1;
    SET v_NewId = CONCAT('BU', LPAD(v_MaxId, 3, '0'));
    INSERT INTO BU_Master(BUId, BUName, BUShortName, Status, CreatedBy, CreatedDate)
    VALUES(v_NewId, p_BUName, p_BUShortName, p_Status, p_CreatedBy, NOW());
    -- UNLOCK TABLES;
    SELECT v_NewId AS NewBUId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_BU_Update(
    IN p_BUId VARCHAR(10), IN p_BUName VARCHAR(200), IN p_BUShortName VARCHAR(50),
    IN p_Status VARCHAR(10), IN p_UpdatedBy INT)
BEGIN
    UPDATE BU_Master
    SET BUName = p_BUName, BUShortName = p_BUShortName, Status = p_Status,
        UpdatedBy = p_UpdatedBy, UpdatedDate = NOW()
    WHERE BUId = p_BUId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_BU_CheckDuplicate(IN p_BUName VARCHAR(200), IN p_BUId VARCHAR(10))
BEGIN
    SELECT COUNT(*) AS Cnt FROM BU_Master
    WHERE BUName = p_BUName AND (p_BUId IS NULL OR BUId <> p_BUId);
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_BU_Dropdown()
BEGIN
    SELECT BUId, BUName FROM BU_Master WHERE Status = 'Active' ORDER BY BUName;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_BU_Audit_Insert(
    IN p_BUId VARCHAR(10), IN p_ActionType VARCHAR(20),
    IN p_ChangedBy INT, IN p_ChangeSummary TEXT)
BEGIN
    INSERT INTO BU_Audit_Log(BUId, ActionType, ChangedBy, ChangeSummary)
    VALUES(p_BUId, p_ActionType, p_ChangedBy, p_ChangeSummary);
END$$
DELIMITER ;

-- ============================================================
-- RISK CATEGORY
-- ============================================================

DELIMITER $$
CREATE PROCEDURE sp_RiskCategory_Search(IN p_Search VARCHAR(100), IN p_Status VARCHAR(10))
BEGIN
    SELECT RiskCatId, RiskCatName, RiskCatAlias, Status FROM RiskCategory_Master
    WHERE (p_Search IS NULL OR p_Search = ''
           OR RiskCatName  LIKE CONCAT('%', p_Search, '%')
           OR RiskCatAlias LIKE CONCAT('%', p_Search, '%'))
      AND (p_Status IS NULL OR p_Status = '' OR Status = p_Status)
    ORDER BY RiskCatName;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskCategory_GetById(IN p_RiskCatId INT)
BEGIN
    SELECT RiskCatId, RiskCatName, RiskCatAlias, Status
    FROM RiskCategory_Master WHERE RiskCatId = p_RiskCatId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskCategory_Insert(
    IN p_RiskCatName VARCHAR(100), IN p_RiskCatAlias VARCHAR(100),
    IN p_Status VARCHAR(10), IN p_CreatedBy INT)
BEGIN
    INSERT INTO RiskCategory_Master(RiskCatName, RiskCatAlias, Status, CreatedBy, CreatedDate)
    VALUES(p_RiskCatName, p_RiskCatAlias, p_Status, p_CreatedBy, NOW());
    SELECT LAST_INSERT_ID() AS NewId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskCategory_Update(
    IN p_RiskCatId INT, IN p_RiskCatName VARCHAR(100), IN p_RiskCatAlias VARCHAR(100),
    IN p_Status VARCHAR(10), IN p_UpdatedBy INT)
BEGIN
    UPDATE RiskCategory_Master
    SET RiskCatName = p_RiskCatName, RiskCatAlias = p_RiskCatAlias, Status = p_Status,
        UpdatedBy = p_UpdatedBy, UpdatedDate = NOW()
    WHERE RiskCatId = p_RiskCatId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskCategory_CheckDuplicate(IN p_RiskCatName VARCHAR(100), IN p_RiskCatId INT)
BEGIN
    SELECT COUNT(*) AS Cnt FROM RiskCategory_Master
    WHERE RiskCatName = p_RiskCatName AND (p_RiskCatId IS NULL OR RiskCatId <> p_RiskCatId);
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskCategory_Dropdown(IN p_Mode VARCHAR(10))
BEGIN
    IF p_Mode = 'ACTIVE' THEN
        SELECT RiskCatId, RiskCatName, Status FROM RiskCategory_Master
        WHERE Status = 'Active' ORDER BY RiskCatName;
    ELSE
        SELECT RiskCatId,
               CASE WHEN Status = 'Inactive' THEN CONCAT(RiskCatName, ' (Inactive)') ELSE RiskCatName END AS RiskCatName,
               Status
        FROM RiskCategory_Master
        ORDER BY CASE WHEN Status = 'Active' THEN 1 ELSE 2 END, RiskCatName;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskCategory_Audit_Insert(
    IN p_RiskCatId INT, IN p_ActionType VARCHAR(20),
    IN p_ChangedBy INT, IN p_ChangeSummary TEXT)
BEGIN
    INSERT INTO RiskCategory_Audit_Log(RiskCatId, ActionType, ChangedBy, ChangeSummary)
    VALUES(p_RiskCatId, p_ActionType, p_ChangedBy, p_ChangeSummary);
END$$
DELIMITER ;

-- ============================================================
-- RISK SUB-CATEGORY
-- ============================================================

DELIMITER $$
CREATE PROCEDURE sp_RiskSubCategory_Search(
    IN p_Search VARCHAR(100), IN p_Status VARCHAR(10), IN p_RiskCatId INT)
BEGIN
    SELECT s.RiskSubCatId, s.RiskCatId, c.RiskCatName,
           s.RiskSubCatName, s.RiskCatAlias, s.Status
    FROM RiskSubCategory_Master s
    INNER JOIN RiskCategory_Master c ON s.RiskCatId = c.RiskCatId
    WHERE (p_Search IS NULL OR p_Search = ''
           OR s.RiskSubCatName LIKE CONCAT('%', p_Search, '%')
           OR s.RiskCatAlias   LIKE CONCAT('%', p_Search, '%'))
      AND (p_Status IS NULL OR p_Status = '' OR s.Status = p_Status)
      AND (p_RiskCatId IS NULL OR p_RiskCatId = 0 OR s.RiskCatId = p_RiskCatId)
    ORDER BY c.RiskCatName, s.RiskSubCatName;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskSubCategory_GetById(IN p_Id INT)
BEGIN
    SELECT * FROM RiskSubCategory_Master WHERE RiskSubCatId = p_Id;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskSubCategory_Insert(
    IN p_RiskCatId INT, IN p_RiskSubCatName VARCHAR(100), IN p_RiskCatAlias VARCHAR(100),
    IN p_Status VARCHAR(10), IN p_CreatedBy INT)
BEGIN
    INSERT INTO RiskSubCategory_Master(RiskCatId, RiskSubCatName, RiskCatAlias, Status, CreatedBy, CreatedDate)
    VALUES(p_RiskCatId, p_RiskSubCatName, p_RiskCatAlias, p_Status, p_CreatedBy, NOW());
    SELECT LAST_INSERT_ID() AS NewId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskSubCategory_Update(
    IN p_Id INT, IN p_RiskCatId INT, IN p_RiskSubCatName VARCHAR(100),
    IN p_RiskCatAlias VARCHAR(100), IN p_Status VARCHAR(10), IN p_UpdatedBy INT)
BEGIN
    UPDATE RiskSubCategory_Master
    SET RiskCatId = p_RiskCatId, RiskSubCatName = p_RiskSubCatName, RiskCatAlias = p_RiskCatAlias,
        Status = p_Status, UpdatedBy = p_UpdatedBy, UpdatedDate = NOW()
    WHERE RiskSubCatId = p_Id;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskSubCategory_CheckDuplicate(IN p_RiskCatId INT, IN p_Name VARCHAR(100), IN p_Id INT)
BEGIN
    SELECT COUNT(*) AS Cnt FROM RiskSubCategory_Master
    WHERE RiskCatId = p_RiskCatId AND RiskSubCatName = p_Name
      AND (p_Id IS NULL OR RiskSubCatId <> p_Id);
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskSubCategory_Dropdown(IN p_RiskCatId INT, IN p_Mode VARCHAR(10))
BEGIN
    IF p_Mode = 'ACTIVE' THEN
        SELECT RiskSubCatId, RiskSubCatName, Status FROM RiskSubCategory_Master
        WHERE RiskCatId = p_RiskCatId AND Status = 'Active'
        ORDER BY RiskSubCatName;
    ELSE
        SELECT RiskSubCatId,
               CASE WHEN Status = 'Inactive' THEN CONCAT(RiskSubCatName, ' (Inactive)') ELSE RiskSubCatName END AS RiskSubCatName,
               Status
        FROM RiskSubCategory_Master
        WHERE RiskCatId = p_RiskCatId
        ORDER BY CASE WHEN Status = 'Active' THEN 1 ELSE 2 END, RiskSubCatName;
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_RiskSubCategory_Audit_Insert(
    IN p_RiskSubCatId INT, IN p_ActionType VARCHAR(20),
    IN p_ChangedBy INT, IN p_ChangeSummary TEXT)
BEGIN
    INSERT INTO RiskSubCategory_Audit_Log(RiskSubCatId, ActionType, ChangedBy, ChangedDate, ChangeSummary)
    VALUES(p_RiskSubCatId, p_ActionType, p_ChangedBy, NOW(), p_ChangeSummary);
END$$
DELIMITER ;

-- ============================================================
-- FUNCTION MASTER
-- ============================================================

DELIMITER $$
CREATE PROCEDURE sp_Function_Search(IN p_Search VARCHAR(200), IN p_Status VARCHAR(10))
BEGIN
    SELECT FunctionId, FunctionName, Status FROM Function_Master
    WHERE (p_Search IS NULL OR p_Search = ''
           OR FunctionName LIKE CONCAT('%', p_Search, '%'))
      AND (p_Status IS NULL OR p_Status = '' OR Status = p_Status)
    ORDER BY FunctionName;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Function_GetById(IN p_FunctionId INT)
BEGIN
    SELECT * FROM Function_Master WHERE FunctionId = p_FunctionId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Function_Insert(IN p_FunctionName VARCHAR(200), IN p_Status VARCHAR(10), IN p_CreatedBy INT)
BEGIN
    INSERT INTO Function_Master(FunctionName, Status, CreatedBy, CreatedDate)
    VALUES(p_FunctionName, p_Status, p_CreatedBy, NOW());
    SELECT LAST_INSERT_ID() AS NewId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Function_Update(
    IN p_FunctionId INT, IN p_FunctionName VARCHAR(200),
    IN p_Status VARCHAR(10), IN p_UpdatedBy INT)
BEGIN
    UPDATE Function_Master
    SET FunctionName = p_FunctionName, Status = p_Status,
        UpdatedBy = p_UpdatedBy, UpdatedDate = NOW()
    WHERE FunctionId = p_FunctionId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Function_CheckDuplicate(IN p_FunctionName VARCHAR(200), IN p_FunctionId INT)
BEGIN
    SELECT COUNT(*) AS Cnt FROM Function_Master
    WHERE FunctionName = p_FunctionName AND (p_FunctionId IS NULL OR FunctionId <> p_FunctionId);
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Function_Dropdown()
BEGIN
    SELECT FunctionId, FunctionName FROM Function_Master WHERE Status = 'Active' ORDER BY FunctionName;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Function_Audit_Insert(
    IN p_FunctionId INT, IN p_ActionType VARCHAR(20),
    IN p_ChangedBy INT, IN p_ChangeSummary TEXT)
BEGIN
    INSERT INTO Function_Audit_Log(FunctionId, ActionType, ChangedBy, ChangeSummary)
    VALUES(p_FunctionId, p_ActionType, p_ChangedBy, p_ChangeSummary);
END$$
DELIMITER ;

-- ============================================================
-- FY DROPDOWN
-- ============================================================

DELIMITER $$
CREATE PROCEDURE sp_FY_Dropdown()
BEGIN
    SELECT FYId, FYName FROM FY_Master ORDER BY FYId DESC;
END$$
DELIMITER ;

-- ============================================================
-- USER PERMISSIONS
-- ============================================================

DELIMITER $$
CREATE PROCEDURE sp_UserPermission_ByUser(IN p_UserId INT)
BEGIN
    SELECT b.BUId,
           CASE WHEN b.Status = 'Inactive' THEN CONCAT(b.BUName, ' (Inactive)') ELSE b.BUName END AS BUName,
           b.Status,
           p.PermissionId,
           IFNULL(p.Role, 'N') AS Role
    FROM BU_Master b
    LEFT JOIN UserPermission p ON p.BUId = b.BUId AND p.UserId = p_UserId
    ORDER BY CASE WHEN b.Status = 'Active' THEN 1 ELSE 2 END, b.BUName;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_UserPermission_Insert(
    IN p_UserId INT, IN p_BUId VARCHAR(10), IN p_Role CHAR(1), IN p_CreatedBy INT)
BEGIN
    INSERT INTO UserPermission(UserId, BUId, Role, CreatedBy, CreatedDate)
    VALUES(p_UserId, p_BUId, p_Role, p_CreatedBy, NOW());
    SELECT LAST_INSERT_ID() AS NewId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_UserPermission_UpdateRole(
    IN p_PermissionId INT, IN p_NewRole CHAR(1), IN p_ChangedBy INT)
BEGIN
    UPDATE UserPermission SET Role = p_NewRole WHERE PermissionId = p_PermissionId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_UserPermission_CheckDuplicate(IN p_UserId INT, IN p_BUId VARCHAR(10))
BEGIN
    SELECT COUNT(*) AS Cnt FROM UserPermission WHERE UserId = p_UserId AND BUId = p_BUId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_UserPermission_GetRole(IN p_PermissionId INT)
BEGIN
    SELECT Role FROM UserPermission WHERE PermissionId = p_PermissionId;
END$$
DELIMITER ;

-- ============================================================
-- RISK MANAGEMENT
-- ============================================================

DELIMITER $$
CREATE PROCEDURE sp_Risk_Insert(
    IN p_BUId VARCHAR(10), IN p_FYId VARCHAR(20),
    IN p_RiskCatId INT, IN p_RiskSubCatId INT, IN p_FunctionId INT,
    IN p_RiskTitle VARCHAR(300), IN p_Description TEXT,
    IN p_ImpactLevel VARCHAR(10), IN p_Likelihood VARCHAR(10),
    IN p_CreatedBy INT)
BEGIN
    INSERT INTO Risk_Master(BUId, FYId, RiskCatId, RiskSubCatId, FunctionId,
        RiskTitle, Description, ImpactLevel, Likelihood, Status, CreatedBy, CreatedDate)
    VALUES(p_BUId, p_FYId, p_RiskCatId, p_RiskSubCatId, p_FunctionId,
        p_RiskTitle, p_Description, p_ImpactLevel, p_Likelihood, 'Draft', p_CreatedBy, NOW());
    SELECT LAST_INSERT_ID() AS NewRiskId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Risk_GetById(IN p_RiskId INT)
BEGIN
    SELECT r.*,
           b.BUName, f.FYName,
           rc.RiskCatName, rs.RiskSubCatName,
           fn.FunctionName,
           u.FullName  AS CreatedByName,
           rv.FullName AS ReviewedByName
    FROM Risk_Master r
    LEFT JOIN BU_Master              b  ON r.BUId         = b.BUId
    LEFT JOIN FY_Master              f  ON r.FYId         = f.FYId
    LEFT JOIN RiskCategory_Master    rc ON r.RiskCatId    = rc.RiskCatId
    LEFT JOIN RiskSubCategory_Master rs ON r.RiskSubCatId = rs.RiskSubCatId
    LEFT JOIN Function_Master        fn ON r.FunctionId   = fn.FunctionId
    LEFT JOIN User_Master            u  ON r.CreatedBy    = u.UserId
    LEFT JOIN User_Master            rv ON r.ReviewedBy   = rv.UserId
    WHERE r.RiskId = p_RiskId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Risk_Search(
    IN p_UserId INT, IN p_AdminFlag VARCHAR(10),
    IN p_BUId VARCHAR(10), IN p_FYId VARCHAR(20),
    IN p_RiskCatId INT, IN p_Status VARCHAR(30), IN p_Search VARCHAR(200))
BEGIN
    SELECT r.RiskId, r.RiskTitle, r.ImpactLevel, r.Likelihood, r.Status,
           r.CreatedDate, b.BUName, f.FYName, rc.RiskCatName, rs.RiskSubCatName
    FROM Risk_Master r
    LEFT JOIN BU_Master              b  ON r.BUId         = b.BUId
    LEFT JOIN FY_Master              f  ON r.FYId         = f.FYId
    LEFT JOIN RiskCategory_Master    rc ON r.RiskCatId    = rc.RiskCatId
    LEFT JOIN RiskSubCategory_Master rs ON r.RiskSubCatId = rs.RiskSubCatId
    WHERE (p_AdminFlag = 'Y'
           OR EXISTS (SELECT 1 FROM UserPermission up
                      WHERE up.UserId = p_UserId AND up.BUId = r.BUId AND up.Role <> 'N'))
      AND (p_BUId IS NULL OR p_BUId = '' OR r.BUId = p_BUId)
      AND (p_FYId IS NULL OR p_FYId = '' OR r.FYId = p_FYId)
      AND (p_RiskCatId IS NULL OR p_RiskCatId = 0 OR r.RiskCatId = p_RiskCatId)
      AND (p_Status IS NULL OR p_Status = '' OR r.Status = p_Status)
      AND (p_Search IS NULL OR p_Search = ''
           OR r.RiskTitle   LIKE CONCAT('%', p_Search, '%')
           OR r.Description LIKE CONCAT('%', p_Search, '%'))
    ORDER BY r.CreatedDate DESC;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Risk_Update(
    IN p_RiskId INT, IN p_BUId VARCHAR(10), IN p_FYId VARCHAR(20),
    IN p_RiskCatId INT, IN p_RiskSubCatId INT, IN p_FunctionId INT,
    IN p_RiskTitle VARCHAR(300), IN p_Description TEXT,
    IN p_ImpactLevel VARCHAR(10), IN p_Likelihood VARCHAR(10), IN p_UpdatedBy INT)
BEGIN
    UPDATE Risk_Master
    SET BUId = p_BUId, FYId = p_FYId, RiskCatId = p_RiskCatId,
        RiskSubCatId = p_RiskSubCatId, FunctionId = p_FunctionId,
        RiskTitle = p_RiskTitle, Description = p_Description,
        ImpactLevel = p_ImpactLevel, Likelihood = p_Likelihood,
        UpdatedBy = p_UpdatedBy, UpdatedDate = NOW()
    WHERE RiskId = p_RiskId AND Status IN ('Draft', 'RevisionRequired');
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Risk_UpdateStatus(
    IN p_RiskId INT, IN p_NewStatus VARCHAR(30),
    IN p_Remarks TEXT, IN p_ChangedBy INT)
BEGIN
    UPDATE Risk_Master
    SET Status      = p_NewStatus,
        Remarks     = p_Remarks,
        ReviewedBy  = CASE WHEN p_NewStatus IN ('Approved','Rejected','RevisionRequired') THEN p_ChangedBy ELSE ReviewedBy END,
        ReviewedDate = CASE WHEN p_NewStatus IN ('Approved','Rejected','RevisionRequired') THEN NOW() ELSE ReviewedDate END,
        UpdatedBy   = p_ChangedBy,
        UpdatedDate = NOW()
    WHERE RiskId = p_RiskId;
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Risk_History_Insert(
    IN p_RiskId INT, IN p_OldStatus VARCHAR(30), IN p_NewStatus VARCHAR(30),
    IN p_Action VARCHAR(50), IN p_Remarks TEXT, IN p_ChangedBy INT)
BEGIN
    INSERT INTO Risk_History(RiskId, OldStatus, NewStatus, Action, Remarks, ChangedBy)
    VALUES(p_RiskId, p_OldStatus, p_NewStatus, p_Action, p_Remarks, p_ChangedBy);
END$$
DELIMITER ;

DELIMITER $$
CREATE PROCEDURE sp_Risk_History_GetByRisk(IN p_RiskId INT)
BEGIN
    SELECT h.HistoryId, h.OldStatus, h.NewStatus, h.Action, h.Remarks,
           h.ChangedDate, u.FullName AS ChangedByName
    FROM Risk_History h
    LEFT JOIN User_Master u ON h.ChangedBy = u.UserId
    WHERE h.RiskId = p_RiskId
    ORDER BY h.ChangedDate DESC;
END$$
DELIMITER ;

-- ============================================================
-- DASHBOARD
-- ============================================================

DELIMITER $$
CREATE PROCEDURE sp_Dashboard_Summary(IN p_UserId INT, IN p_AdminFlag VARCHAR(10))
BEGIN
    -- Result set 1: Total count
    SELECT COUNT(*) AS TotalRisks
    FROM Risk_Master r
    WHERE p_AdminFlag = 'Y'
       OR EXISTS (SELECT 1 FROM UserPermission up
                  WHERE up.UserId = p_UserId AND up.BUId = r.BUId AND up.Role <> 'N');

    -- Result set 2: By Status
    SELECT r.Status, COUNT(*) AS Cnt
    FROM Risk_Master r
    WHERE p_AdminFlag = 'Y'
       OR EXISTS (SELECT 1 FROM UserPermission up
                  WHERE up.UserId = p_UserId AND up.BUId = r.BUId AND up.Role <> 'N')
    GROUP BY r.Status;

    -- Result set 3: By BU
    SELECT b.BUName, COUNT(*) AS Cnt
    FROM Risk_Master r
    JOIN BU_Master b ON r.BUId = b.BUId
    WHERE p_AdminFlag = 'Y'
       OR EXISTS (SELECT 1 FROM UserPermission up
                  WHERE up.UserId = p_UserId AND up.BUId = r.BUId AND up.Role <> 'N')
    GROUP BY b.BUName ORDER BY Cnt DESC;

    -- Result set 4: By Category
    SELECT rc.RiskCatName, COUNT(*) AS Cnt
    FROM Risk_Master r
    JOIN RiskCategory_Master rc ON r.RiskCatId = rc.RiskCatId
    WHERE p_AdminFlag = 'Y'
       OR EXISTS (SELECT 1 FROM UserPermission up
                  WHERE up.UserId = p_UserId AND up.BUId = r.BUId AND up.Role <> 'N')
    GROUP BY rc.RiskCatName ORDER BY Cnt DESC;

    -- Result set 5: High impact alerts (not closed)
    SELECT r.RiskId, r.RiskTitle, r.Status, r.ImpactLevel,
           b.BUName, rc.RiskCatName, r.CreatedDate
    FROM Risk_Master r
    JOIN BU_Master           b  ON r.BUId      = b.BUId
    JOIN RiskCategory_Master rc ON r.RiskCatId = rc.RiskCatId
    WHERE r.ImpactLevel = 'High' AND r.Status <> 'Closed'
      AND (p_AdminFlag = 'Y'
           OR EXISTS (SELECT 1 FROM UserPermission up
                      WHERE up.UserId = p_UserId AND up.BUId = r.BUId AND up.Role <> 'N'))
    ORDER BY r.CreatedDate DESC LIMIT 10;

    -- Result set 6: By FY
    SELECT r.FYId, f.FYName, COUNT(*) AS Cnt
    FROM Risk_Master r
    JOIN FY_Master f ON r.FYId = f.FYId
    WHERE p_AdminFlag = 'Y'
       OR EXISTS (SELECT 1 FROM UserPermission up
                  WHERE up.UserId = p_UserId AND up.BUId = r.BUId AND up.Role <> 'N')
    GROUP BY r.FYId, f.FYName ORDER BY r.FYId DESC;
END$$
DELIMITER ;

-- ============================================================
-- RISK WIZARD EXTENSION PROCEDURES (Steps 2–5)
-- ============================================================

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

DELIMITER $$
CREATE PROCEDURE sp_RiskPeople_GetByRisk(IN p_RiskId INT)
BEGIN
    SELECT
        rp.Id, rp.RiskId, rp.OwnerId,
        o.FullName AS OwnerName, o.Username AS OwnerUsername,
        rp.ChampionId,
        c.FullName AS ChampionName, c.Username AS ChampionUsername
    FROM Risk_PeopleOverview rp
    LEFT JOIN User_Master o ON rp.OwnerId    = o.UserId
    LEFT JOIN User_Master c ON rp.ChampionId = c.UserId
    WHERE rp.RiskId = p_RiskId;
END$$
DELIMITER ;

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

DELIMITER $$
CREATE PROCEDURE sp_RiskAssessment_GetByRisk(IN p_RiskId INT)
BEGIN
    SELECT
        ra.Id, ra.RiskId, ra.ResponseMeasure, ra.CurrentControls,
        ra.LineOfAction, ra.ResponsibleTeam, ra.TargetDate,
        ra.FrequencyId, af.AFName AS FrequencyName
    FROM Risk_Assessment ra
    LEFT JOIN ActionFrequency_Master af ON ra.FrequencyId = af.AFId
    WHERE ra.RiskId = p_RiskId;
END$$
DELIMITER ;

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

DELIMITER $$
CREATE PROCEDURE sp_RiskAttachment_Delete(IN p_AttachmentId INT, IN p_RiskId INT)
BEGIN
    DELETE FROM Risk_Attachments
    WHERE Id = p_AttachmentId AND RiskId = p_RiskId;
END$$
DELIMITER ;

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

