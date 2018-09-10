CREATE PROC dbo.spBulkInsertForImportProfile AS
BEGIN
	
	DECLARE @LoginIDTable TABLE(ID INT, rowID INT IDENTITY(1,1))

	-- INSERT INTO Login Table

	INSERT INTO tblLogin
	(
		UserName,
		Password,
		Address,
		EmailId,
		City,
		State,
		ZipCode,
		IsAdmin
	) OUTPUT inserted.ID INTO @LoginIDTable(ID)
	SELECT
		UserName,
		Password,
		Address,
		Email,
		City,
		State,
		CAST(ZipCode AS INT) AS ZipCode,
		CAST(CASE WHEN UPPER(IsAdmin) = 'TRUE' THEN 1 ELSE 0 END AS BIT) AS IsAdmin
	FROM
	ImportProfile



	UPDATE A
	SET A.Login_ID = B.ID
	FROM ImportProfile A
	JOIN @LoginIDTable B ON B.rowID = A.ID

	-- INSERT INTO Consumer TABLE

	INSERT INTO tblConsumer(
		ConsumerNo,
		ConsumerName,
		RegionCode,
		Login_ID
	)
	SELECT
		CAST(ConsumerNo AS INT),
		UserName,
		RegionCode,
		Login_ID
	FROM
	ImportProfile



END