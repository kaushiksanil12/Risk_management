USE `risk_management_db`;

DELIMITER $$
CREATE TRIGGER `TRG_UserPermission_Insert`
AFTER INSERT ON `UserPermission` FOR EACH ROW
BEGIN
    INSERT INTO `UserPermission_Log`(PermissionId, UserId, BUId, OldRole, NewRole, ActionType, ChangedBy)
    VALUES(NEW.PermissionId, NEW.UserId, NEW.BUId, NULL, NEW.Role, 'INSERT', NEW.CreatedBy);
END$$
DELIMITER ;

DELIMITER $$
CREATE TRIGGER `TRG_UserPermission_Update`
AFTER UPDATE ON `UserPermission` FOR EACH ROW
BEGIN
    IF OLD.`Role` <> NEW.`Role` THEN
        INSERT INTO `UserPermission_Log`(PermissionId, UserId, BUId, OldRole, NewRole, ActionType, ChangedBy)
        VALUES(NEW.PermissionId, NEW.UserId, NEW.BUId, OLD.Role, NEW.Role, 'UPDATE', NEW.CreatedBy);
    END IF;
END$$
DELIMITER ;

DELIMITER $$
CREATE TRIGGER `TRG_UserPermission_Delete`
AFTER DELETE ON `UserPermission` FOR EACH ROW
BEGIN
    INSERT INTO `UserPermission_Log`(PermissionId, UserId, BUId, OldRole, NewRole, ActionType, ChangedBy)
    VALUES(OLD.PermissionId, OLD.UserId, OLD.BUId, OLD.Role, NULL, 'DELETE', OLD.CreatedBy);
END$$
DELIMITER ;
