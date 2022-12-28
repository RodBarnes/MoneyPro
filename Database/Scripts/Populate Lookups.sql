INSERT INTO AccountType ([ImportName],[DisplayName]) VALUES ('Bank','Bank');
INSERT INTO AccountType ([ImportName],[DisplayName]) VALUES ('Cash','Cash');
INSERT INTO AccountType ([ImportName],[DisplayName]) VALUES ('CCard','Credit Card');
INSERT INTO AccountType ([ImportName],[DisplayName]) VALUES ('Invst','Investment');
INSERT INTO AccountType ([ImportName],[DisplayName]) VALUES ('Oth A','Asset');
INSERT INTO AccountType ([ImportName],[DisplayName]) VALUES ('Oth L','Liabilty');
GO

INSERT INTO InvestmentAction ([Name]) VALUES ('Buy')
INSERT INTO InvestmentAction ([Name]) VALUES ('Sell')
INSERT INTO InvestmentAction ([Name]) VALUES ('Dividend')
INSERT INTO InvestmentAction ([Name]) VALUES ('Interest')
INSERT INTO InvestmentAction ([Name]) VALUES ('Other Income')
INSERT INTO InvestmentAction ([Name]) VALUES ('Other Expense')
INSERT INTO InvestmentAction ([Name]) VALUES ('Return Capital')
INSERT INTO InvestmentAction ([Name]) VALUES ('Reinvest Dividend')
INSERT INTO InvestmentAction ([Name]) VALUES ('Reinvest Interest')
INSERT INTO InvestmentAction ([Name]) VALUES ('Add Shares')
INSERT INTO InvestmentAction ([Name]) VALUES ('Remove Shares')
INSERT INTO InvestmentAction ([Name]) VALUES ('Transfer In')
INSERT INTO InvestmentAction ([Name]) VALUES ('Transfer Out')
INSERT INTO InvestmentAction ([Name]) VALUES ('Transfer In Short')
INSERT INTO InvestmentAction ([Name]) VALUES ('Transfer Out Short')
INSERT INTO InvestmentAction ([Name]) VALUES ('Short Sell')
INSERT INTO InvestmentAction ([Name]) VALUES ('Cover Short')
INSERT INTO InvestmentAction ([Name]) VALUES ('Redeem CD/Bond')
INSERT INTO InvestmentAction ([Name]) VALUES ('Renew CD')
INSERT INTO InvestmentAction ([Name]) VALUES ('Buy to Open')
INSERT INTO InvestmentAction ([Name]) VALUES ('Buy to Close')
INSERT INTO InvestmentAction ([Name]) VALUES ('Sell to Open')
INSERT INTO InvestmentAction ([Name]) VALUES ('Sell to Close')
GO

INSERT INTO Payee ([Name], [DateLastUsed]) VALUES ('Opening Balance', NULL)
INSERT INTO Payee ([Name], [DateLastUsed]) VALUES ('Interest Earned', NULL)
GO

INSERT INTO Category ([Text], Tax) VALUES ('Transfer', 0)
GO

INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Weekly',7,'day')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Every other week',14,'day')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Twice a month',.5,'month')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Every four weeks',28,'day')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Monthly',1,'month')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Every other month',2,'month')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Every three months',3,'month')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Every four months',4,'month')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Twice a year',6,'month')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Yearly',1,'year')
INSERT INTO ScheduleRule ([Name],[Number],[Interval]) VALUES('Every other Year',2,'year')
GO
