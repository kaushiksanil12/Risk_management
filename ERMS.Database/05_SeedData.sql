USE `risk_management_db`;

-- Fiscal Years
INSERT INTO `FY_Master`(`FYId`,`FYName`,`FromDate`,`ToDate`) VALUES
('2025-26','FY 2025-26','2025-04-01','2026-03-31'),
('2026-27','FY 2026-27','2026-04-01','2027-03-31');

-- Seed Users  (admin / Admin@123  |  riskuser1 / User@123)
INSERT INTO `User_Master`(`Username`,`LoginType`,`Password`,`FullName`,`Email`,`Mobile`,`Status`,`AdminFlag`) VALUES
('admin',     'Custom',   SHA2('Admin@123',256), 'System Administrator','admin@erm.com',    '9999999991','Active','Y'),
('riskuser1', 'Custom',   SHA2('User@123', 256), 'Risk Manager 1',      'risk1@erm.com',    '9999999992','Active','N'),
('riskuser2', 'LDAP_TMG', NULL,                  'Risk Manager 2',      'risk2@erm.com',    '9999999993','Active','N');

-- Business Units
INSERT INTO `BU_Master`(`BUId`,`BUName`,`BUShortName`,`Status`) VALUES
('BU001','Westtek Unit 3','Westtek','Active'),
('BU002','Manipal Payment and Identity Solutions (MPi)','MPi','Active'),
('BU003','Adsyndicate','Adsy','Active'),
('BU004','Manipal Business Solutions (MBS)','MBS','Active'),
('BU005','Manipal Digital Solutions (MDS)','MDS','Active'),
('BU006','Ekam','Ekam','Active'),
('BU007','Primacy','Primacy','Active'),
('BU008','Manipal Fintech Private Ltd (MFPL)','MFPL','Active'),
('BU009','MGPS','MGPS','Active'),
('BU010','Manipal Energy & Infratech Ltd (MEIL)','MEIL','Active'),
('BU011','MTL Corporate','MTL','Active'),
('BU012','MMNL','MMNL','Active');

-- Risk Categories
INSERT INTO `RiskCategory_Master`(`RiskCatName`,`RiskCatAlias`,`Status`) VALUES
('Strategic','Strategic','Active'),
('Business and Market Risk','BusinessMarket','Active'),
('Employee','Employee','Active'),
('Operations','Operations','Active'),
('Financial','Financial','Active'),
('IT Risk and Compliance','ITCompliance','Active');

-- Risk Sub-Categories (RiskCatId 1 = Strategic)
INSERT INTO `RiskSubCategory_Master`(`RiskCatId`,`RiskSubCatName`,`Status`) VALUES
(1,'Reputational Risk (Brand, CSR)','Active'),(1,'Enterprise Vision','Active'),
(1,'Focus Business for Growth','Active'),(1,'Timing Business Decision','Active'),
(1,'Business Continuity Risk','Active'),(1,'Business Life Cycle','Active');

-- (RiskCatId 2 = Business and Market Risk)
INSERT INTO `RiskSubCategory_Master`(`RiskCatId`,`RiskSubCatName`,`Status`) VALUES
(2,'Contract Management','Active'),(2,'Pricing','Active'),(2,'Concentration Risk','Active'),
(2,'Competition Risk','Active'),(2,'Product Design & Development','Active'),
(2,'Business Cycle','Active'),(2,'Customer and Supplier Relation','Active'),
(2,'Marketing Strategy','Active');

-- (RiskCatId 3 = Employee)
INSERT INTO `RiskSubCategory_Master`(`RiskCatId`,`RiskSubCatName`,`Status`) VALUES
(3,'Leadership Availability','Active'),(3,'Talent Management Risk','Active'),
(3,'Succession Plan Risk','Active'),(3,'Employee Conduct Risk','Active'),
(3,'Organization Structure','Active'),(3,'Corporate Culture','Active'),
(3,'Embezzlement','Active'),(3,'Employee Morale','Active');

-- (RiskCatId 4 = Operations)
INSERT INTO `RiskSubCategory_Master`(`RiskCatId`,`RiskSubCatName`,`Status`) VALUES
(4,'Operations Risk','Active'),(4,'Infra / Tech Risk','Active'),
(4,'Compliance Risk','Active'),(4,'Revenue Risk','Active'),
(4,'Vendor Risk','Active'),(4,'Fraud','Active'),(4,'Natural Calamities','Active');

-- (RiskCatId 5 = Financial)
INSERT INTO `RiskSubCategory_Master`(`RiskCatId`,`RiskSubCatName`,`Status`) VALUES
(5,'Fund Management and Liquidity Risk','Active'),(5,'Credit and Forex Risk','Active'),
(5,'Financial Control and Reporting','Active'),(5,'Cost Structure and Profitability','Active'),
(5,'Macro Economic Environment','Active'),(5,'Interest Rate Cycle','Active'),
(5,'Third Party Liability','Active');

-- (RiskCatId 6 = IT Risk and Compliance)
INSERT INTO `RiskSubCategory_Master`(`RiskCatId`,`RiskSubCatName`,`Status`) VALUES
(6,'Information and Cyber Security','Active'),(6,'GDPR and Data Privacy Risk','Active'),
(6,'IT Infringement Risk','Active'),(6,'Opensource Software Risk','Active'),
(6,'Operational Hazard','Active'),(6,'Compliance Risk','Active'),
(6,'Infrastructure Security','Active'),(6,'Network Security','Active'),
(6,'Access Management','Active'),(6,'Incident Management','Active'),
(6,'Business Continuity / Disaster Recovery','Active');

-- Functions
INSERT INTO `Function_Master`(`FunctionName`,`Status`) VALUES
('Business','Active'),('Finance','Active'),('Human Resources','Active'),
('Legal','Active'),('Marketing','Active'),('EHS','Active'),
('IT & Cyber Security','Active'),('Plant Operations','Active'),
('Supply Chain','Active'),('Operations','Active');

-- Action Statuses
INSERT INTO `ActionStatus_Master`(`StatusId`,`StatusName`,`DisplayFlag`) VALUES
('NS','Not Started','Y'),('IP','In Progress','Y'),('CM','Completed','Y');

-- Action Frequencies
INSERT INTO `ActionFrequency_Master`(`AFId`,`AFName`,`Days`,`Status`) VALUES
('MNT','Monthly',30,'Active'),('QTR','Quarterly',91,'Active'),('YER','Yearly',365,'Active');
