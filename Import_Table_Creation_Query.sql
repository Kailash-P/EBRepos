CREATE TABLE dbo.ImportProfile(
	UserName VARCHAR(20) DEFAULT('') NOT NULL,
	Password VARCHAR(20) DEFAULT('') NOT NULL,
	ConsumerNo VARCHAR(MAX) DEFAULT('') NOT NULL,
	RegionCode VARCHAR(MAX) DEFAULT('') NOT NULL,
	Address VARCHAR(MAX) DEFAULT('') NOT NULL,
	Email VARCHAR(50) DEFAULT('') NOT NULL,
	City VARCHAR(20) DEFAULT('') NOT NULL,
	State VARCHAR(20) DEFAULT('') NOT NULL,
	ZipCode VARCHAR(MAX) DEFAULT('') NOT NULL,
	IsAdmin VARCHAR(MAX) DEFAULT('') NOT NULL,
	Login_ID INT DEFAULT(0) NOT NULL,
	ID INT IDENTITY(1,1)
)



CREATE TABLE dbo.ImportError(
	ErrorLevel VARCHAR(MAX) DEFAULT('') NOT NULL,
	ColumnName VARCHAR(MAX) DEFAULT('') NOT NULL,
	ErrorMessage VARCHAR(MAX) DEFAULT('') NOT NULL,
	ID INT DEFAULT(0) NOT NULL
)


