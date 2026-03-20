-- Stored procedure to get email recipients for risk notifications
-- Result set 1: Risk Owner details + risk info
-- Result set 2: All active Champions for the BU of this risk

DELIMITER $$
CREATE PROCEDURE IF NOT EXISTS sp_Risk_GetEmailRecipients(IN p_RiskId INT)
BEGIN
    -- Result set 1: Risk Owner details + risk info
    SELECT
        u.Email    AS OwnerEmail,
        u.FullName AS OwnerName,
        r.RiskTitle,
        r.BUId,
        b.BUName,
        r.Status
    FROM Risk_Master r
    JOIN User_Master u ON r.CreatedBy = u.UserId
    JOIN BU_Master   b ON r.BUId      = b.BUId
    WHERE r.RiskId = p_RiskId;

    -- Result set 2: All active Champions for the BU of this risk
    SELECT
        u.Email    AS ChampionEmail,
        u.FullName AS ChampionName
    FROM UserPermission p
    JOIN User_Master    u ON p.UserId = u.UserId
    JOIN Risk_Master    r ON p.BUId   = r.BUId
    WHERE r.RiskId  = p_RiskId
      AND p.Role    = 'C'
      AND u.Status  = 'Active';
END$$
DELIMITER ;
