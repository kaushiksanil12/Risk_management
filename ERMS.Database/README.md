# ERMS Database Scripts

## Overview
These SQL scripts create and populate the ERM_DB database for the Enterprise Risk Management System.

## Execution Order
Run scripts in order (01 → 05) against the MariaDB Docker container:

```bash
docker exec -i risk_management_db mysql -u riskadmin -pRiskAdmin@123 < 01_CreateDatabase.sql
docker exec -i risk_management_db mysql -u riskadmin -pRiskAdmin@123 ERM_DB < 02_CreateTables.sql
docker exec -i risk_management_db mysql -u riskadmin -pRiskAdmin@123 ERM_DB < 03_CreateProcedures.sql
docker exec -i risk_management_db mysql -u riskadmin -pRiskAdmin@123 ERM_DB < 04_CreateTriggers.sql
docker exec -i risk_management_db mysql -u riskadmin -pRiskAdmin@123 ERM_DB < 05_SeedData.sql
```

## Contents
| Script | Description |
|--------|------------|
| 01_CreateDatabase.sql | Creates ERM_DB database |
| 02_CreateTables.sql | Creates all 18 tables |
| 03_CreateProcedures.sql | Creates all stored procedures |
| 04_CreateTriggers.sql | Creates 3 audit triggers |
| 05_SeedData.sql | Seeds reference data and default users |

## Default Logins
| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | System Administrator |
| riskuser1 | User@123 | Standard User |
| riskuser2 | (LDAP) | Standard User (LDAP) |
