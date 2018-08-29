--CREATE DATABASE ElectricityBoardDatabase

USE ElectricityBoardDatabase

--CREATE TABLE dbo.tblLogin
--(
--	ID INT IDENTITY(1,1) PRIMARY KEY(ID),
--	UserName VARCHAR(20) DEFAULT('') NOT NULL,
--	Password VARCHAR(20) DEFAULT('') NOT NULL 
--)


--ALTER TABLE tblLogin ADD Address VARCHAR(MAX) DEFAULT('') NOT NULL

--ALTER TABLE tblLogin ADD EmailId VARCHAR(50) DEFAULT('') NOT NULL

--INSERT INTO tblLogin
--SELECT 'admin','admin'

--CREATE TABLE dbo.tblConsumer
--(
--	ID INT IDENTITY(1,1) PRIMARY KEY (ID),
--	ConsumerNo INT DEFAULT(0) NOT NULL,
--	ConsumerName VARCHAR(20) DEFAULT('') NOT NULL,
--	RegionCode VARCHAR(20) DEFAULT('') NOT NULL,
--	Login_ID INT DEFAULT(0) FOREIGN KEY REFERENCES tblLogin(ID)
--)

--INSERT INTO tblConsumer
--SELECT 01108043206,'Kailash','01-North Chennai',1

--CREATE TABLE dbo.tblBillHistory
--(
--	ID INT IDENTITY(1,1) PRIMARY KEY (ID),
--	BillDate DATETIME DEFAULT(0) NOT NULL,
--	UnitsConsumed INT DEFAULT(0) NOT NULL,
--	BillAmount INT DEFAULT(0) NOT NULL,
--	Paid BIT DEFAULT(0) NOT NULL,
--	Consumer_ID INT DEFAULT(0) FOREIGN KEY REFERENCES tblConsumer(ID)
--)

--ALTER TABLE tblBillHistory ADD BillPaidDate DATETIME DEFAULT(0) NOT NULL

--INSERT INTO tblBillHistory
--SELECT GETDATE(),1000,1000,0,1




SELECT * FROM tblLogin

SELECT * FROM tblConsumer

SELECT * FROM tblBillHistory
